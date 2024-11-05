using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Controllers;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using SchoolAPI.Exceptions;
using SchoolAPI.Constants;

public class StudentControllerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;
    private readonly Faker<Student> _studentFaker;
    private readonly Faker<StudentPostDTO> _studentPostDtoFaker;

    public StudentControllerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _studentRepositoryMock = new Mock<IStudentRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentGetDTO>();
            cfg.CreateMap<StudentPostDTO, Student>();
        });
        _mapper = config.CreateMapper();

        _controller = new StudentController(_studentServiceMock.Object, _studentRepositoryMock.Object, _mapper);

        _studentFaker = new Faker<Student>()
            .RuleFor(s => s.StudentId, f => f.IndexFaker + 1)
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.BirthDate, f => f.Date.Past(18))
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.IsActive, true);

        _studentPostDtoFaker = new Faker<StudentPostDTO>()
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.BirthDate, f => f.Date.Past(18))
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber());
    }

    [Fact]
    public async Task Post_ShouldReturnOkResult_WhenStudentIsCreated()
    {
        // Arrange
        var studentPostDTO = _studentPostDtoFaker.Generate();
        var student = _mapper.Map<Student>(studentPostDTO);
        var createdStudent = _studentFaker.Generate();
        createdStudent.StudentId = 1;


        _studentRepositoryMock.Setup(r => r.DuplicateEntriesChecker(It.IsAny<Student>())).ReturnsAsync(false);
        _studentServiceMock.Setup(s => s.GetAge(studentPostDTO.BirthDate)).Returns(18);
        _studentRepositoryMock.Setup(r => r.CreateStudentAsync(It.IsAny<Student>())).ReturnsAsync(createdStudent);

        // Act
        var result = await _controller.Post(studentPostDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(createdStudent.StudentId, returnedStudent.StudentId);
        Assert.Equal(createdStudent.FirstName, returnedStudent.FirstName);
        Assert.Equal(createdStudent.LastName, returnedStudent.LastName);
        Assert.Equal(createdStudent.Email, returnedStudent.Email);
        Assert.Equal(createdStudent.Phone, returnedStudent.Phone);
    }

    [Fact]
    public async Task Post_ShouldThrowException_WhenStudentAlreadyExists()
    {
        // Arrange
        var studentPostDTO = _studentPostDtoFaker.Generate();
        var student = _mapper.Map<Student>(studentPostDTO);

        _studentRepositoryMock.Setup(r => r.DuplicateEntriesChecker(It.IsAny<Student>())).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.Post(studentPostDTO));
        Assert.Equal(ErrorMessages.STUDENT_EXISTS, exception.Message);
    }

    [Fact]
    public async Task GetAllStudentAsync_ShouldReturnOkResultWithPaginatedList()
    {
        // Arrange
        var students = _studentFaker.Generate(5);
        _studentRepositoryMock.Setup(r => r.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((students, 5));

        // Act
        var result = await _controller.GetAllStudentAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var allStudents = Assert.IsType<GetAllStudentsDTO>(okResult.Value);
        Assert.Equal(5, allStudents.StudentList.Count());
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnOkResult_WhenStudentExists()
    {
        // Arrange
        var student = _studentFaker.Generate();
        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(student.StudentId)).ReturnsAsync(student);

        // Act
        var result = await _controller.GetStudentById(student.StudentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.StudentId, returnedStudent.StudentId);
        Assert.Equal(student.FirstName, returnedStudent.FirstName);
        Assert.Equal(student.LastName, returnedStudent.LastName);
        Assert.Equal(student.Email, returnedStudent.Email);
        Assert.Equal(student.Phone, returnedStudent.Phone);
    }

    [Fact]
    public async Task GetStudentById_ShouldThrowStudentNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() => _controller.GetStudentById(1));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }

    [Fact]
    public async Task Put_ShouldReturnOkResult_WhenStudentIsUpdated()
    {
        // Arrange
        var studentUpdateDTO = new StudentUpdateDTO { StudentId = 1, FirstName = "UpdatedName", Email = "updated@example.com" };
        var oldStudent = _studentFaker.Generate();
        oldStudent.StudentId = studentUpdateDTO.StudentId;
        oldStudent.Email = "original@example.com";

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(studentUpdateDTO.StudentId)).ReturnsAsync(oldStudent);

        // Act
        var result = await _controller.Put(studentUpdateDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(studentUpdateDTO.FirstName, updatedStudent.FirstName);
        Assert.Equal(studentUpdateDTO.Email, updatedStudent.Email);
        Assert.Equal(oldStudent.LastName, updatedStudent.LastName);
    }

    [Fact]
    public async Task Put_ShouldThrowException_WhenNoFieldsAreUpdated()
    {
        // Arrange
        var studentUpdateDTO = new StudentUpdateDTO { StudentId = 1 };
        var oldStudent = _studentFaker.Generate();
        oldStudent.StudentId = studentUpdateDTO.StudentId;

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(studentUpdateDTO.StudentId)).ReturnsAsync(oldStudent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.Put(studentUpdateDTO));
        Assert.Equal(ErrorMessages.NOTHING_TO_UPDATE, exception.Message);
    }

    [Fact]
    public async Task Delete_ShouldReturnOkResult_WhenStudentIsDeactivated()
    {
        // Arrange
        var student = _studentFaker.Generate();
        student.StudentId = 1;

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(student.StudentId)).ReturnsAsync(student);

        // Act
        var result = await _controller.Delete(student.StudentId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.False(student.IsActive);
    }

    [Fact]
    public async Task Delete_ShouldThrowStudentNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() => _controller.Delete(1));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }
}

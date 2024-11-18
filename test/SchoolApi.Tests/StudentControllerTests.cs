using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Controllers;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using CommonLibrary.Constants;
using CommonLibrary.Exceptions;
using SchoolApi.Core.Tests.Helper;

public class StudentControllerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;
    private readonly FakeDataCreator _dataCreator;

    public StudentControllerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _studentRepositoryMock = new Mock<IStudentRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentGetDTO>().ReverseMap();
            cfg.CreateMap<StudentPostDTO, Student>().ReverseMap();
            cfg.CreateMap<StudentUpdateDTO, Student>().ReverseMap();
        });
        _mapper = config.CreateMapper();

        _controller = new StudentController(_studentServiceMock.Object, _studentRepositoryMock.Object, _mapper);
        
        _dataCreator = new FakeDataCreator();
    }

    [Fact]
    public async Task Post_ShouldReturnOkResult_WhenStudentIsCreated()
    {
        // Arrange
        var createdStudent = _dataCreator.StudentFaker.Generate();
        var studentPostDTO = _mapper.Map<StudentPostDTO>(createdStudent);

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

    // [Fact]
    // public async Task Post_ShouldThrowCustomException_WhenStudentAlreadyExists()
    // {
    //     // Arrange
    //     var student = _dataCreator.StudentFaker.Generate();
    //     var studentPostDTO = _mapper.Map<StudentPostDTO>(student);

    //     _studentRepositoryMock.Setup(r => r.DuplicateEntriesChecker(It.IsAny<Student>())).ReturnsAsync(true);

    //     // Act & Assert
    //     var exception = await Assert.ThrowsAsync<CustomException>(() => _controller.Post(studentPostDTO));
    //     Assert.Equal(ErrorMessages.StudentExistsExceptionDetails, exception.Message);
    // }

    [Fact]
    public async Task Post_ShouldThrowCustomException_WhenStudentAlreadyExists()
    {
        // Arrange
        var student = _dataCreator.StudentFaker.Generate();
        var studentPostDTO = _mapper.Map<StudentPostDTO>(student);

        _studentRepositoryMock.Setup(r => r.DuplicateEntriesChecker(It.IsAny<Student>())).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _controller.Post(studentPostDTO));
        var expectedErrorDetails = ErrorMessages.StudentExistsExceptionDetails;
        Assert.Equal(expectedErrorDetails.Message, exception.ErrorDetails.Message);
        Assert.Equal(expectedErrorDetails.StatusCode, exception.ErrorDetails.StatusCode);
    }

    [Fact]
    public async Task GetAllStudentAsync_ShouldReturnOkResultWithPaginatedList()
    {
        // Arrange
        var students = _dataCreator.StudentFaker.Generate(5);
        _studentRepositoryMock.Setup(r => r.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                              .ReturnsAsync((students, 5));

        // Act
        var result = await _controller.GetAllStudentAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var allStudents = Assert.IsType<GetAllStudentsDTO>(okResult.Value);

        Assert.Equal(5, allStudents.StudentList.Count());

        var studentList = allStudents.StudentList.ToList();
        for (int i = 0; i < students.Count; i++)
        {
            Assert.Equal(students[i].StudentId, studentList[i].StudentId);
            Assert.Equal(students[i].FirstName, studentList[i].FirstName);
            Assert.Equal(students[i].LastName, studentList[i].LastName);
            Assert.Equal(students[i].Email, studentList[i].Email);
            Assert.Equal(students[i].Phone, studentList[i].Phone);
            Assert.Equal(students[i].BirthDate, studentList[i].BirthDate);
            Assert.Equal(students[i].Age, studentList[i].Age);
        }

        Assert.Equal(5, allStudents.TotalCount);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnOkResult_WhenStudentExists()
    {
        // Arrange
        var student = _dataCreator.StudentFaker.Generate();
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
        Assert.Equal(student.BirthDate, returnedStudent.BirthDate);
        Assert.Equal(student.Age, returnedStudent.Age);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        // Act
        var result = await _controller.GetStudentById(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ErrorMessages.StudentNotFoundExceptionDetails, notFoundResult.Value);
    }

    [Fact]
    public async Task Put_ShouldReturnOkResult_WhenStudentIsUpdated()
    {
        // Arrange
        var oldStudent = _dataCreator.StudentFaker.Generate();
        var studentUpdateDTO = _mapper.Map<StudentUpdateDTO>(_dataCreator.StudentFaker.Generate());
        oldStudent.StudentId = studentUpdateDTO.StudentId;

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(studentUpdateDTO.StudentId)).ReturnsAsync(oldStudent);

        // Act
        var result = await _controller.Put(studentUpdateDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(oldStudent.StudentId, updatedStudent.StudentId);
        Assert.Equal(studentUpdateDTO.LastName, updatedStudent.LastName);
        Assert.Equal(studentUpdateDTO.Email, updatedStudent.Email);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenNoFieldsAreUpdated()
    {
        // Arrange
        var studentUpdateDTO = new StudentUpdateDTO { StudentId = 1 };
        var oldStudent = _dataCreator.StudentFaker.Generate();
        oldStudent.StudentId = studentUpdateDTO.StudentId;

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(studentUpdateDTO.StudentId)).ReturnsAsync(oldStudent);

        // Act
        var result = await _controller.Put(studentUpdateDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ErrorMessages.NothingToUpdate, badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnOkResult_WhenStudentIsDeactivated()
    {
        // Arrange
        var student = _dataCreator.StudentFaker.Generate();

        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(student.StudentId)).ReturnsAsync(student);

        // Act
        var result = await _controller.Delete(student.StudentId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.False(student.IsActive);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        _studentRepositoryMock.Setup(r => r.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ErrorMessages.StudentNotFoundExceptionDetails, notFoundResult.Value);
    }
}

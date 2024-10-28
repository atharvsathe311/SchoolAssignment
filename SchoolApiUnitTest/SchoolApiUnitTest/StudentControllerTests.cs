using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolApi.Core.DTO;
using SchoolApi.Core.Service;
using SchoolAPI.Controllers;
using SchoolAPI.Validators;

namespace SchoolApiUnitTest
{
    public class StudentControllerTests
    {

        private readonly StudentPostDTOValidator _validator;
        private readonly StudentController _sut;
        private readonly Mock<IStudentService> _studentServiceMock = new Mock<IStudentService>();
        private readonly Mock<IValidator<StudentPostDTO>> _validatorMock = new Mock<IValidator<StudentPostDTO>>();

        public StudentControllerTests()
        {
            _sut = new StudentController(_studentServiceMock.Object, _validatorMock.Object);
            _validator = new StudentPostDTOValidator();
        }

        [Fact]
        public async Task Post_ShouldReturnStudentDTO_WhenStudentIsCreated()
        {
            // Arrange
            var studentPostDTO = new StudentPostDTO
            {
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01")
            };

            var studentGetDTO = new StudentGetDTO
            {
                StudentId = 1,
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24,
            };

            _validatorMock.Setup(x => x.ValidateAsync(studentPostDTO,default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _studentServiceMock.Setup(x => x.CreateStudentAsync(It.IsAny<StudentPostDTO>())).ReturnsAsync(studentGetDTO);

            // Act
            var result = await _sut.Post(studentPostDTO);
            
            // Assert

            _studentServiceMock.Verify(x => x.CreateStudentAsync(It.IsAny<StudentPostDTO>()), Times.Once);
            _validatorMock.Verify(x => x.ValidateAsync(studentPostDTO,default), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<StudentGetDTO>(okResult.Value);
            StudentGetDTO returnedStudent = (StudentGetDTO)okResult.Value;

            Assert.Equal(studentGetDTO.StudentId, returnedStudent.StudentId);
            Assert.Equal(studentGetDTO.FirstName, returnedStudent.FirstName);
            Assert.Equal(studentGetDTO.LastName, returnedStudent.LastName);
            Assert.Equal(studentGetDTO.Email, returnedStudent.Email);
            Assert.Equal(studentGetDTO.Phone, returnedStudent.Phone);
            Assert.Equal(studentGetDTO.BirthDate, returnedStudent.BirthDate);
            Assert.Equal(studentGetDTO.Age, returnedStudent.Age);

        }

        [Fact]
        public void FirstName_ShouldHaveValidationErrors_WhenInvalid()
        {
            // Arrange
            var student = new StudentPostDTO { FirstName = "" }; // Empty FirstName

            // Act
            var result = _validator.TestValidate(student);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.FirstName)
                .WithErrorMessage("FirstName is Required");

            student.FirstName = "A";
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.FirstName)
                .WithErrorMessage("FirstName must be atleast 2 Characters");

            student.FirstName = "VeryLongFirstName";
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.FirstName)
                .WithErrorMessage("FirstName cannot exceed 15 Characters");
        }

        [Fact]
        public void LastName_ShouldHaveValidationErrors_WhenInvalid()
        {
            // Arrange
            var student = new StudentPostDTO { LastName = "" };

            // Act
            var result = _validator.TestValidate(student);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.LastName)
                .WithErrorMessage("LastName is Required");

            student.LastName = "B";
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.LastName)
                .WithErrorMessage("LastName must be atleast 2 Characters");

            student.LastName = "VeryLongLastName";
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.LastName)
                .WithErrorMessage("LastName cannot exceed 15 Characters");
        }

        [Fact]
        public void Email_ShouldHaveValidationErrors_WhenInvalid()
        {
            // Arrange
            var student = new StudentPostDTO { Email = "" };

            // Act
            var result = _validator.TestValidate(student);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.Email)
                .WithErrorMessage("Email is Required");

            student.Email = "invalid-email"; 
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.Email)
                .WithErrorMessage("Invalid Email");
        }

        [Fact]
        public void Phone_ShouldHaveValidationErrors_WhenInvalid()
        {
            // Arrange
            var student = new StudentPostDTO { Phone = "" }; 

            // Act
            var result = _validator.TestValidate(student);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.Phone)
                .WithErrorMessage("Phone Number is Required");

            student.Phone = "1234567890";
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.Phone)
                .WithErrorMessage("Invalid Phone Number");
        }

        [Fact]
        public void BirthDate_ShouldHaveValidationErrors_WhenInvalid()
        {
            // Arrange
            var student = new StudentPostDTO { BirthDate = DateTime.Now };

            // Act
            var result = _validator.TestValidate(student);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.BirthDate)
                .WithErrorMessage("Invalid Birthdate");

            student.BirthDate = DateTime.Now.AddYears(-5);
            result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.BirthDate)
                .WithErrorMessage("Invalid Birthdate");

            student.BirthDate = DateTime.Now.AddYears(-11);
            result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.BirthDate);
        }
    
        [Fact]
        public async Task Post_ShouldReturnServerError_WhenServiceFails()
        {
            // Arrange
            var studentPostDTO = new StudentPostDTO
            {
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01")
            };

            _validatorMock.Setup(x => x.ValidateAsync(studentPostDTO, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _studentServiceMock.Setup(x => x.CreateStudentAsync(It.IsAny<StudentPostDTO>())).ThrowsAsync(new Exception("Any Exception"));

            // Act
            var result = await _sut.Post(studentPostDTO);

            // Assert
            _studentServiceMock.Verify(x => x.CreateStudentAsync(It.IsAny<StudentPostDTO>()), Times.Once);
            _validatorMock.Verify(x => x.ValidateAsync(studentPostDTO,default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Any Exception", objectResult.Value);
        }
    
        [Fact]
        public async Task GetAllStudents_ShouldReturnOkResult_WithStudentList_WhenStudentsExist()
        {
            // Arrange
            var students = new List<StudentGetDTO>
            {
                new() { 
                    StudentId = 1, 
                    FirstName = "Dhruv", 
                    LastName = "Trivedi", 
                    Email = "dhruvtrivedi@gmail.com", 
                    Phone = "1234567890", 
                    BirthDate = DateTime.Parse("2000-01-01"), 
                    Age = 24 
                },
                new() { 
                    StudentId = 2, 
                    FirstName = "Avadhoot", 
                    LastName = "Virkar", 
                    Email = "avadhoot@gmail.com", 
                    Phone = "0987654321", 
                    BirthDate = DateTime.Parse("2000-01-01"), 
                    Age = 24 
                }
            };

            var totalCount = students.Count;

            _studentServiceMock.Setup(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((students, totalCount));

            // Act
            var result = await _sut.GetAllStudentAsync();

            // Assert

            _studentServiceMock.Verify(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var (StudentsList, TotalCount) = Assert.IsType<(List<StudentGetDTO> StudentsList, int TotalCount)>(okResult.Value);

            Assert.Equal(totalCount, TotalCount);
            Assert.Equal(totalCount, StudentsList.Count());

            var returnedStudents = StudentsList.ToList();

            for (int i = 0; i < students.Count; i++)
            {
                Assert.Equal(students[i].StudentId, returnedStudents[i].StudentId);
                Assert.Equal(students[i].FirstName, returnedStudents[i].FirstName);
                Assert.Equal(students[i].LastName, returnedStudents[i].LastName);
                Assert.Equal(students[i].Email, returnedStudents[i].Email);
                Assert.Equal(students[i].Phone, returnedStudents[i].Phone);
                Assert.Equal(students[i].BirthDate, returnedStudents[i].BirthDate);
                Assert.Equal(students[i].Age, returnedStudents[i].Age);
            }
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnOkResult_WithEmptyList_WhenNoStudentsExist()
        {
            // Arrange
            var students = new List<StudentGetDTO>();
            var totalCount = 0;

            _studentServiceMock.Setup(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((students, totalCount));

            // Act
            var result = await _sut.GetAllStudentAsync();

            // Assert

            _studentServiceMock.Verify(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<(List<StudentGetDTO> StudentsList, int TotalCount)>(okResult.Value);

            Assert.Equal(totalCount, returnedValue.TotalCount);
            Assert.Empty(returnedValue.StudentsList);
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturn500StatusCode_WhenExceptionOccurs()
        {
            // Arrange
            _studentServiceMock.Setup(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _sut.GetAllStudentAsync();

            // Assert

            _studentServiceMock.Verify(x => x.GetAllStudentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error", statusCodeResult.Value?.ToString());
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnOkResult_WithStudent_WhenStudentExists()
        {
            // Arrange
            int studentId = 1;
            var studentGetDTO = new StudentGetDTO
            {
                StudentId = 1,
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24,
            };

            _studentServiceMock.Setup(x => x.GetStudentByIdAsync(studentId)).ReturnsAsync(studentGetDTO);

            // Act
            var result = await _sut.GetStudentById(studentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
            
            Assert.Equal(studentGetDTO.StudentId, returnedStudent.StudentId);
            Assert.Equal(studentGetDTO.FirstName, returnedStudent.FirstName);
            Assert.Equal(studentGetDTO.LastName, returnedStudent.LastName);
            Assert.Equal(studentGetDTO.Email, returnedStudent.Email);
            Assert.Equal(studentGetDTO.Phone, returnedStudent.Phone);
            Assert.Equal(studentGetDTO.BirthDate, returnedStudent.BirthDate);
            Assert.Equal(studentGetDTO.Age, returnedStudent.Age);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            int studentId = 1;
            _studentServiceMock.Setup(x => x.GetStudentByIdAsync(studentId)).ThrowsAsync(new Exception("Student Not Found"));

            // Act
            var result = await _sut.GetStudentById(studentId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Student Not Found", statusResult.Value);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            int studentId = 1;
            _studentServiceMock.Setup(x => x.GetStudentByIdAsync(studentId)).ThrowsAsync(new Exception("Some server error"));

            // Act
            var result = await _sut.GetStudentById(studentId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Some server error", statusResult.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnOkResult_WithUpdatedStudent_WhenUpdatesAreMade()
        {
            // Arrange
            int studentId = 1;
            var studentPostDTO = new StudentPostDTO
            {
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01")
            };

            var studentGetDTO = new StudentGetDTO
            {
                StudentId = studentId,
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24
            };

            _validatorMock.Setup(x => x.ValidateAsync(studentPostDTO,default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _studentServiceMock.Setup(x => x.UpdateStudentAsync(studentId, studentPostDTO)).ReturnsAsync(studentGetDTO);

            // Act
            var result = await _sut.Put(studentId, studentPostDTO);

            // Assert
            _studentServiceMock.Verify(x => x.UpdateStudentAsync(studentId, studentPostDTO), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);

            Assert.Equal(studentGetDTO.StudentId, updatedStudent.StudentId);
            Assert.Equal(studentGetDTO.FirstName, updatedStudent.FirstName);
            Assert.Equal(studentGetDTO.LastName, updatedStudent.LastName);
            Assert.Equal(studentGetDTO.BirthDate, updatedStudent.BirthDate);
            Assert.Equal(studentGetDTO.Email, updatedStudent.Email);
            Assert.Equal(studentGetDTO.Phone, updatedStudent.Phone);
            Assert.Equal(studentGetDTO.Age, updatedStudent.Age);
        }

        [Fact]
        public async Task Put_ShouldReturnOkResult_WithUnchangedStudent_WhenNoUpdatesAreMade()
        {
            // Arrange
            var studentId = 1;
            var studentPostDTO = new StudentPostDTO
            {
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01")
            };

            var updatedStudent = new StudentGetDTO
            {
                StudentId = studentId,
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24
            };

            _validatorMock.Setup(x => x.ValidateAsync(studentPostDTO,default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _studentServiceMock.Setup(x => x.UpdateStudentAsync(studentId, studentPostDTO)).ReturnsAsync(updatedStudent);

            // Act
            var result = await _sut.Put(studentId, studentPostDTO);

            // Assert
            _studentServiceMock.Verify(x => x.UpdateStudentAsync(studentId, studentPostDTO), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var unchangedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);

            Assert.Equal(updatedStudent.StudentId, unchangedStudent.StudentId);
            Assert.Equal(updatedStudent.FirstName, unchangedStudent.FirstName);
            Assert.Equal(updatedStudent.LastName, unchangedStudent.LastName);
            Assert.Equal(updatedStudent.BirthDate, unchangedStudent.BirthDate);
            Assert.Equal(updatedStudent.Email, unchangedStudent.Email);
            Assert.Equal(updatedStudent.Phone, unchangedStudent.Phone);
            Assert.Equal(updatedStudent.Age, unchangedStudent.Age);
        }

        [Fact]
        public async Task Put_ShouldReturnInternalServerError_WhenStudentDoesNotExist()
        {
            // Arrange
            var studentId = 1;
            var studentPostDTO = new StudentPostDTO
            {
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01")
            };

            var updatedStudent = new StudentGetDTO
            {
                StudentId = studentId,
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24
            };

            _validatorMock.Setup(x => x.ValidateAsync(studentPostDTO,default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _studentServiceMock.Setup(x => x.UpdateStudentAsync(studentId, studentPostDTO)).ThrowsAsync(new Exception("Student Not Found"));

            // Act
            var result = await _sut.Put(studentId, studentPostDTO);

            // Assert
            _studentServiceMock.Verify(x => x.UpdateStudentAsync(studentId, studentPostDTO), Times.Once);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Student Not Found", statusResult.Value);
        }

         [Fact]
        public async Task Delete_ShouldReturnOkResult_WhenStudentIsDeletedSuccessfully()
        {
            // Arrange
            int studentId = 1;

            _studentServiceMock
                .Setup(x => x.DeleteStudentAsync(studentId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.Delete(studentId);

            // Assert
            _studentServiceMock.Verify(x => x.DeleteStudentAsync(studentId), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnInternalServerError_WhenStudentNotFound()
        {
            // Arrange
            int studentId = 1;

            _studentServiceMock
                .Setup(x => x.DeleteStudentAsync(studentId))
                .ThrowsAsync(new Exception("Student Not Found"));

            // Act
            var result = await _sut.Delete(studentId);

            // Assert
            _studentServiceMock.Verify(x => x.DeleteStudentAsync(studentId), Times.Once);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Student Not Found", statusResult.Value);
        }
    
    }
}

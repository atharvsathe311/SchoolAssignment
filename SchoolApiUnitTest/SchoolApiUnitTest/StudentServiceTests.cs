using AutoMapper;
using Moq;
using SchoolAPI.DTO;
using SchoolAPI.Models;
using SchoolAPI.Repository;
using SchoolAPI.Service;

namespace SchoolApiUnitTest
{
    public class StudentServiceTests
    {
        private readonly StudentService _sut;
        private readonly Mock<IStudentRepository> _studentRepositoryMock = new Mock<IStudentRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public StudentServiceTests()
        {
            _sut = new StudentService(_studentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateStudentAsync_ShouldReturnStudentDTO_WhenStudentIsCreated()
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

            var student = new Student
            {
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
            };

            var studentToReturn = new Student
            {
                StudentId = 1,
                FirstName = "Dhruv",
                LastName = "Trivedi",
                Email = "dhruvtrivedi@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 24,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsActive = true
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

            _mapperMock.Setup(x => x.Map<Student>(studentPostDTO)).Returns(student);
            _studentRepositoryMock.Setup(x => x.CreateStudentAsync(It.IsAny<Student>())).ReturnsAsync(studentToReturn);
            _mapperMock.Setup(x => x.Map<StudentGetDTO>(studentToReturn)).Returns(studentGetDTO);

            // Act
            var result = await _sut.CreateStudentAsync(studentPostDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(studentGetDTO,result);

            _studentRepositoryMock.Verify(x => x.CreateStudentAsync(It.IsAny<Student>()), Times.Once);
            _mapperMock.Verify(x => x.Map<Student>(studentPostDTO), Times.Once);
            _mapperMock.Verify(x => x.Map<StudentGetDTO>(studentToReturn), Times.Once);
        }

        [Fact]
        public async Task GetAllStudentAsync_ShouldReturnPaginatedList_WhenStudentsExist()
        {
                // Arrange
            var students = new List<Student>
            {
                new Student
                {
                    StudentId = 1,
                    FirstName = "Saket",
                    LastName = "Shah",
                    BirthDate = DateTime.Parse("2000-01-01"),
                    Email = "saket@gmail.com",
                    Phone = "0987654321",
                    Age = 24,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    IsActive = true
                },
                new Student
                {
                    StudentId = 2,
                    FirstName = "Avadhoot",
                    LastName = "Virkar",
                    BirthDate = DateTime.Parse("2000-01-01"),
                    Email = "avadhoot@gmail.com",
                    Phone = "0987654321",
                    Age = 24,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    IsActive = true
                }
            };

            var listToReturn = students.Select(s => new StudentGetDTO
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Phone = s.Phone,
                BirthDate = s.BirthDate,
                Age = s.Age
            }).ToList();

            _studentRepositoryMock.Setup(x => x.GetAllStudentAsync(1, 10, ""))
                .ReturnsAsync((students.AsEnumerable(), students.Count));

            _mapperMock.Setup(x => x.Map<IEnumerable<StudentGetDTO>>(students))
                .Returns(listToReturn);

            // Act
            var result = await _sut.GetAllStudentAsync(1, 10, "");

            // Assert
            Assert.NotNull(result);
            var resultData = result.GetType().GetProperty("StudentsList")?.GetValue(result) as IEnumerable<StudentGetDTO>;
            Assert.NotNull(resultData);            
            Assert.Equal(listToReturn.Count, resultData.Count());
            Assert.True(listToReturn.SequenceEqual(resultData), "The lists do not match.");
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            var studentId = 1;
            var firstName = "Atharv";
            var lastName = "Sathe";

            var student = new Student
            {
                StudentId = studentId,
                FirstName = firstName,
                LastName = lastName,
                Email = "atharvsathe11@gmail.com",
                Phone = "9404465615",
                BirthDate = DateTime.Parse("2002-11-04"),
                Age = 21,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsActive = true
            };

            var studentDto = new StudentGetDTO
            {
                StudentId = studentId,
                FirstName = firstName,
                LastName = lastName,
                Email = "atharvsathe11@gmail.com",
                Phone = "9404465615",
                BirthDate = DateTime.Parse("2002-11-03"),
                Age = 21
            };

            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId))
                .ReturnsAsync(student);

            _mapperMock.Setup(x => x.Map<StudentGetDTO>(student))
               .Returns(studentDto);

            // Act
            var result = await _sut.GetStudentByIdAsync(studentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(studentDto.StudentId, result.StudentId);
            Assert.Equal(studentDto.FirstName, result.FirstName);
            Assert.Equal(studentDto.LastName, result.LastName);
            Assert.Equal(studentDto.Email, result.Email);
            Assert.Equal(studentDto.Phone, result.Phone);
            Assert.Equal(studentDto.BirthDate, result.BirthDate);
            Assert.Equal(studentDto.Age, result.Age);

            _studentRepositoryMock.Verify(repo => repo.GetStudentByIdAsync(studentId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<StudentGetDTO>(student), Times.Once);
        }

        [Fact]
        public async Task GetStudentByIdAsync_houldThrowException_WhenStudentDoesntExist()
        {
            // Arrange
            var studentId = 1234;
            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId)).ReturnsAsync(() => null);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _sut.GetStudentByIdAsync(studentId));
            
            // Assert
            Assert.Equal("Student Not Found", exception.Message);

            _studentRepositoryMock.Verify(repo => repo.GetStudentByIdAsync(studentId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<StudentGetDTO>(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldReturnUpdatedStudentDTO_WhenStudentExists()
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

            var existingStudent = new Student
            {
                StudentId = studentId,
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 30,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsActive = true
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

            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId))
                .ReturnsAsync(existingStudent);

            _studentRepositoryMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(x => x.Map<StudentGetDTO>(It.IsAny<Student>()))
                .Returns(updatedStudent);

            // Act
            var result = await _sut.UpdateStudentAsync(studentId, studentPostDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedStudent.FirstName, result.FirstName);
            Assert.Equal(updatedStudent.LastName, result.LastName);
            Assert.Equal(updatedStudent.Email, result.Email);
            Assert.Equal(updatedStudent.Phone, result.Phone);
            Assert.Equal(updatedStudent.BirthDate, result.BirthDate);
            Assert.Equal(24, result.Age);

            // Verify repository save was called
            _studentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _studentRepositoryMock.Verify(x => x.GetStudentByIdAsync(studentId), Times.Once);
            _mapperMock.Verify(x => x.Map<StudentGetDTO>(It.IsAny<Student>()), Times.Once);

        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldThrowException_WhenStudentDoesNotExist()
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

            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId)).ReturnsAsync(() => null);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _sut.UpdateStudentAsync(studentId, studentPostDTO));

            // Assert
            Assert.Equal("Student Not Found", exception.Message);

            _studentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
            _studentRepositoryMock.Verify(x => x.GetStudentByIdAsync(studentId), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldDeactivateStudent_WhenStudentExists()
        {
            // Arrange
            var studentId = 1;
            var existingStudent = new Student
            {
                StudentId = studentId,
                FirstName = "Neel",
                LastName = "Dalvi",
                Email = "neel@gmail.com",
                Phone = "1234567890",
                BirthDate = DateTime.Parse("2000-01-01"),
                Age = 30,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsActive = true
            };

            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId))
                .ReturnsAsync(existingStudent);

            _studentRepositoryMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _sut.DeleteStudentAsync(studentId);

            // Assert
            Assert.False(existingStudent.IsActive);
            _studentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldThrowException_WhenStudentDoesNotExist()
        {
            // Arrange
            var studentId = 1;

            _studentRepositoryMock.Setup(x => x.GetStudentByIdAsync(studentId)).ReturnsAsync(() => null);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _sut.DeleteStudentAsync(studentId));
            
            // Assert
            Assert.Equal("Student Not Found", exception.Message);
            _studentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
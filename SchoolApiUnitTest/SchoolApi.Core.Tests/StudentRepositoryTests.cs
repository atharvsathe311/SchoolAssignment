using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Data;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Tests.Helper;

namespace SchoolApiUnitTest
{
    public class StudentRepositoryTests : IDisposable
    {
        private readonly FakeDataCreator _dataCreator;
        private readonly SchoolDbContext _context;
        private readonly StudentRepository _repository;

        public StudentRepositoryTests()
        {
            _dataCreator = new FakeDataCreator();
            _context = GetInMemoryDbContext();
            _repository = new StudentRepository(_context);
        }

        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new SchoolDbContext(options);
            SeedData(context);
            return context;
        }

        private void SeedData(SchoolDbContext context)
        {
            context.Students.AddRange(_dataCreator.StudentFaker.Generate(10));
            context.SaveChanges();
        }

        [Fact]
        public async Task CreateStudentAsync_ShouldAddStudent()
        {
            // Arrange
            var newStudent = _dataCreator.StudentFaker.Generate();
            newStudent.Created = DateTime.UtcNow;
            newStudent.Updated = DateTime.UtcNow;
            newStudent.IsActive = true;

            // Act
            var result = await _repository.CreateStudentAsync(newStudent);
            var students = await _context.Students.ToListAsync();

            // Assert
            Assert.Contains(result, students);
            Assert.Equal(newStudent.FirstName, result.FirstName);
            Assert.Equal(newStudent.LastName, result.LastName);
            Assert.Equal(newStudent.Email, result.Email);
            Assert.Equal(newStudent.Phone, result.Phone);
            Assert.Equal(newStudent.BirthDate, result.BirthDate);
            Assert.Equal(newStudent.Age, result.Age);
            Assert.Equal(newStudent.Created, result.Created);
            Assert.Equal(newStudent.Updated, result.Updated);
            Assert.Equal(newStudent.IsActive, result.IsActive);
        }

        [Fact]
        public async Task GetAllStudentAsync_ShouldReturnActiveStudents_WithPagination()
        {
            // Act
            var (students, count) = await _repository.GetAllStudentAsync(1, 5, "");

            // Assert
            Assert.Equal(_context.Students.Count(s => s.IsActive), count);
            Assert.True(students.Count() <= 5);
            Assert.All(students, s => Assert.True(s.IsActive));
            Assert.All(students, s =>
            {
                Assert.NotNull(s.FirstName);
                Assert.NotNull(s.LastName);
                Assert.NotNull(s.Email);
                Assert.NotNull(s.Phone);
                Assert.NotNull(s.BirthDate);
                Assert.True(s.Age >= 0);
                Assert.True(s.IsActive);
            });
        }

        [Fact]
        public async Task GetAllStudentAsync_ShouldReturnEmpty_WhenNoActiveStudents()
        {
            // Arrange
            _context.Students.RemoveRange(_context.Students);
            await _context.SaveChangesAsync();

            // Act
            var (students, count) = await _repository.GetAllStudentAsync(1, 5, "");

            // Assert
            Assert.Empty(students);
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetAllStudentAsync_WithSearchTerm_ShouldReturnFilteredStudents()
        {
            // Arrange
            var testStudent = _context.Students.FirstOrDefault(s => s.IsActive);
            var searchTerm = testStudent?.FirstName ?? "";

            // Act
            var (students, count) = await _repository.GetAllStudentAsync(1, 10, searchTerm);

            // Assert
            Assert.Contains(students, s => s.FirstName.Contains(searchTerm));
            Assert.All(students, s =>
            {
                Assert.NotNull(s.FirstName);
                Assert.NotNull(s.LastName);
                Assert.NotNull(s.Email);
                Assert.NotNull(s.Phone);
                Assert.NotNull(s.BirthDate);
                Assert.True(s.Age >= 0);
                Assert.True(s.IsActive);
            });
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            var existingStudent = _context.Students.FirstOrDefault();

            // Act
            var student = await _repository.GetStudentByIdAsync(existingStudent.StudentId);

            // Assert
            Assert.NotNull(student);
            Assert.Equal(existingStudent.FirstName, student.FirstName);
            Assert.Equal(existingStudent.LastName, student.LastName);
            Assert.Equal(existingStudent.Email, student.Email);
            Assert.Equal(existingStudent.Phone, student.Phone);
            Assert.Equal(existingStudent.BirthDate, student.BirthDate);
            Assert.Equal(existingStudent.Age, student.Age);
            Assert.Equal(existingStudent.Created, student.Created);
            Assert.Equal(existingStudent.Updated, student.Updated);
            Assert.Equal(existingStudent.IsActive, student.IsActive);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            // Act
            var student = await _repository.GetStudentByIdAsync(999);

            // Assert
            Assert.Null(student);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnTrue_WhenDuplicateEmailExists()
        {
            // Arrange
            var existingStudent = await _context.Students.FirstOrDefaultAsync();

            // Act
            var result = await _repository.DuplicateEntriesChecker(existingStudent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnFalse_WhenNoDuplicateEmailExists()
        {
            // Arrange
            var nonExistingEmailStudent = _dataCreator.StudentFaker.Generate();

            // Act
            var result = await _repository.DuplicateEntriesChecker(nonExistingEmailStudent);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnTrue_WhenDuplicatePhoneExists()
        {
            // Arrange
            var existingStudent = await _context.Students.FirstOrDefaultAsync();

            // Act
            var result = await _repository.DuplicateEntriesChecker(existingStudent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnFalse_WhenNoDuplicatePhoneExists()
        {
            // Arrange
            var nonExistingPhoneStudent = _dataCreator.StudentFaker.Generate();

            // Act
            var result = await _repository.DuplicateEntriesChecker(nonExistingPhoneStudent);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

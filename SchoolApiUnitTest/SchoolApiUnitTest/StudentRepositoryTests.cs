using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Data;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;
using Bogus;

namespace SchoolApiUnitTest
{
    public class StudentRepositoryTests
    {
        private readonly Faker<Student> _studentFaker;

        public StudentRepositoryTests()
        {
            _studentFaker = new Faker<Student>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber("9#########"))
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20))
                .RuleFor(s => s.Age, (f, s) => DateTime.Now.Year - s.BirthDate.Value.Year)
                .RuleFor(s => s.Created, f => f.Date.Past())
                .RuleFor(s => s.Updated, f => f.Date.Recent())
                .RuleFor(s => s.IsActive, f => f.Random.Bool(0.75f));
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
            context.Students.AddRange(_studentFaker.Generate(10));
            context.SaveChanges();
        }

        [Fact]
        public async Task CreateStudentAsync_ShouldAddStudent()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            var newStudent = _studentFaker.Generate();
            newStudent.Created = DateTime.UtcNow;
            newStudent.Updated = DateTime.UtcNow;
            newStudent.IsActive = true;

            // Act
            var result = await repository.CreateStudentAsync(newStudent);
            var students = await context.Students.ToListAsync();

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
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var (students, count) = await repository.GetAllStudentAsync(1, 5, "");

            // Assert
            Assert.Equal(context.Students.Count(s => s.IsActive), count);
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
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            context.Students.RemoveRange(context.Students);
            await context.SaveChangesAsync();

            // Act
            var (students, count) = await repository.GetAllStudentAsync(1, 5, "");

            // Assert
            Assert.Empty(students);
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetAllStudentAsync_WithSearchTerm_ShouldReturnFilteredStudents()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            var testStudent = context.Students.FirstOrDefault(s => s.IsActive);
            var searchTerm = testStudent?.FirstName ?? "";

            // Act
            var (students, count) = await repository.GetAllStudentAsync(1, 10, searchTerm);

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
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            var existingStudent = context.Students.FirstOrDefault();

            // Act
            var student = await repository.GetStudentByIdAsync(existingStudent.StudentId);

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
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var student = await repository.GetStudentByIdAsync(999);

            // Assert
            Assert.Null(student);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnTrue_WhenDuplicateEmailExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);
            var existingStudent = await context.Students.FirstOrDefaultAsync();

            // Act
            var result = await repository.DuplicateEntriesChecker(existingStudent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnFalse_WhenNoDuplicateEmailExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);
            var nonExistingEmailStudent = _studentFaker.Generate();

            // Act
            var result = await repository.DuplicateEntriesChecker(nonExistingEmailStudent);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnTrue_WhenDuplicatePhoneExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);
            var existingStudent = await context.Students.FirstOrDefaultAsync();

            // Act
            var result = await repository.DuplicateEntriesChecker(existingStudent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DuplicateChecker_ShouldReturnFalse_WhenNoDuplicatePhoneExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);
            var nonExistingPhoneStudent = _studentFaker.Generate();

            // Act
            var result = await repository.DuplicateEntriesChecker(nonExistingPhoneStudent);

            // Assert
            Assert.False(result);
        }
    }
}



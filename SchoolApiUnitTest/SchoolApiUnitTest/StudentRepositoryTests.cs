using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolApi.Core.Data;
using SchoolApi.Core.Models;
using SchoolApi.Core.Repository;

namespace SchoolApiUnitTest
{
    public class StudentRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            var context = new SchoolDbContext(options);
            SeedData(context); // Seed test data
            return context;
        }

        private void SeedData(SchoolDbContext context)
        {
            context.Students.AddRange(
                new Student 
                { 
                    StudentId = 1, FirstName = "Atharv", LastName = "Sathe", Email = "atharv@example.com", 
                    Phone = "1234567890", BirthDate = new DateTime(2005, 1, 15), Age = 18, 
                    Created = DateTime.UtcNow.AddMonths(-6), Updated = DateTime.UtcNow.AddDays(-2), 
                    IsActive = true 
                },
                new Student 
                { 
                    StudentId = 2, FirstName = "Dhruv", LastName = "Trivedi", Email = "dhruv@example.com", 
                    Phone = "0987654321", BirthDate = new DateTime(2003, 5, 22), Age = 20, 
                    Created = DateTime.UtcNow.AddMonths(-12), Updated = DateTime.UtcNow.AddDays(-5), 
                    IsActive = true 
                },
                new Student 
                { 
                    StudentId = 3, FirstName = "Avadhoot", LastName = "Virkar", Email = "avadhoot@example.com", 
                    Phone = "1122334455", BirthDate = new DateTime(2004, 3, 17), Age = 19, 
                    Created = DateTime.UtcNow.AddMonths(-8), Updated = DateTime.UtcNow.AddDays(-1), 
                    IsActive = false 
                },
                new Student 
                { 
                    StudentId = 4, FirstName = "Neel", LastName = "Dalvi", Email = "neel@example.com", 
                    Phone = "2233445566", BirthDate = new DateTime(2002, 11, 30), Age = 21, 
                    Created = DateTime.UtcNow.AddMonths(-10), Updated = DateTime.UtcNow.AddDays(-3), 
                    IsActive = true 
                }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task CreateStudentAsync_ShouldAddStudent()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            var newStudent = new Student
            {
                FirstName = "New",
                LastName = "Student",
                Email = "newstudent@example.com",
                Phone = "1231231234",
                BirthDate = new DateTime(2000, 12, 12),
                Age = 22,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                IsActive = true
            };

            // Act
            var result = await repository.CreateStudentAsync(newStudent);
            var students = await context.Students.ToListAsync();

            // Assert
            Assert.Contains(result, students);
            Assert.Equal("New", result.FirstName);
        }

        [Fact]
        public async Task GetAllStudentAsync_ShouldReturnActiveStudents_WithPagination()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var (students, count) = await repository.GetAllStudentAsync(1, 2, "");

            // Assert
            Assert.Equal(3, count); // 3 active students
            Assert.Equal(2, students.Count()); // pageSize of 2
            Assert.All(students, s => Assert.True(s.IsActive)); // Ensure all are active
        }

        [Fact]
        public async Task GetAllStudentAsync_WithSearchTerm_ShouldReturnFilteredStudents()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var (students, count) = await repository.GetAllStudentAsync(1, 10, "Dhruv");

            // Assert
            Assert.Single(students);
            Assert.Equal("Dhruv", students.First().FirstName);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var student = await repository.GetStudentByIdAsync(1);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("Atharv", student.FirstName);
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
    }
}
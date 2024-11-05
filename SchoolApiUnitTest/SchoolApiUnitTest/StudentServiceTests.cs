using SchoolApi.Core.Service;

namespace SchoolApiUnitTest
{
    public class StudentServiceTests
    {
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _studentService = new StudentService();
        }

        [Theory]
        [InlineData("2005-01-15", 19)]
        [InlineData("2000-12-12", 23)]
        [InlineData("1990-11-04", 34)]
        public void GetAge_ShouldReturnCorrectAge(string birthDateString, int expectedAge)
        {
            // Arrange
            DateTime birthDate = DateTime.Parse(birthDateString);

            // Act
            int age = _studentService.GetAge(birthDate);

            // Assert
            Assert.Equal(expectedAge, age);
        }

        [Fact]
        public void GetAge_ShouldSubtractOneYear_WhenBirthdayHasNotOccurredThisYear()
        {
            // Arrange
            DateTime birthDate = DateTime.Now.AddYears(-25).AddDays(1);

            // Act
            int age = _studentService.GetAge(birthDate);

            // Assert
            Assert.Equal(24, age);
        }

        [Fact]
        public void GetAge_ShouldReturnZero_ForBirthDateToday()
        {
            // Arrange
            DateTime birthDate = DateTime.Now;

            // Act
            int age = _studentService.GetAge(birthDate);

            // Assert
            Assert.Equal(0, age);
        }
    }
}

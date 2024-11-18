using Bogus;
using FluentValidation.TestHelper;
using CommonLibrary.Constants;
using SchoolAPI.DTO;
using SchoolAPI.Validators;

namespace SchoolApiUnitTest
{
    public class StudentPostDTOValidatorTest
    {
        private readonly StudentPostDTOValidator _validator;
        private readonly Faker<StudentPostDTO> _studentPostDTOFaker;

        public StudentPostDTOValidatorTest()
        {
            _validator = new StudentPostDTOValidator();

            _studentPostDTOFaker = new Faker<StudentPostDTO>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber("9#########")) 
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20, DateTime.Now.AddYears(-18)));
        }

        // Positive test cases

        [Fact]
        public void Should_NotHaveError_When_AllFieldsAreValid()
        {
            var model = _studentPostDTOFaker.Generate();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // Negative test cases
        [Fact]
        public void Should_HaveError_When_FirstNameIsNull()
        {
            var model = _studentPostDTOFaker.Generate();
            model.FirstName = null;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage(ErrorMessages.FIRST_NAME_REQUIRED);
        }

        [Fact]
        public void Should_HaveError_When_FirstNameIsEmpty()
        {
            var model = _studentPostDTOFaker.Generate();
            model.FirstName = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage(ErrorMessages.FIRST_NAME_REQUIRED);
        }

        [Fact]
        public void Should_HaveError_When_LastNameIsNull()
        {
            var model = _studentPostDTOFaker.Generate();
            model.LastName = null;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.LastName).WithErrorMessage(ErrorMessages.LAST_NAME_REQUIRED);
        }

        [Fact]
        public void Should_HaveError_When_LastNameIsEmpty()
        {
            var model = _studentPostDTOFaker.Generate();
            model.LastName = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.LastName).WithErrorMessage(ErrorMessages.LAST_NAME_REQUIRED);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            var model = _studentPostDTOFaker.Generate();
            model.Email = "invalid-email";
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Email).WithErrorMessage(ErrorMessages.INVALID_EMAIL);
        }

        [Fact]
        public void Should_HaveError_When_PhoneIsNull()
        {
            var model = _studentPostDTOFaker.Generate();
            model.Phone = null;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Phone).WithErrorMessage(ErrorMessages.PHONE_REQUIRED);
        }

        [Fact]
        public void Should_HaveError_When_PhoneIsTooShort()
        {
            var model = _studentPostDTOFaker.Generate();
            model.Phone = "12345";
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Phone).WithErrorMessage(ErrorMessages.INVALID_PHONE_NUMBER);
        }

        [Fact]
        public void Should_HaveError_When_PhoneIsTooLong()
        {
            var model = _studentPostDTOFaker.Generate();
            model.Phone = "123456789012";
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Phone).WithErrorMessage(ErrorMessages.INVALID_PHONE_NUMBER);
        }

        [Fact]
        public void Should_HaveError_When_BirthDateIsInFuture()
        {
            var model = _studentPostDTOFaker.Generate();
            model.BirthDate = DateTime.Now.AddDays(1);
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.BirthDate).WithErrorMessage(ErrorMessages.INVALID_BIRTH_DATE);
        }

        [Fact]
        public void Should_HaveError_When_BirthDateIsUnderage()
        {
            var model = _studentPostDTOFaker.Generate();
            model.BirthDate = DateTime.Now.AddYears(-9);
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.BirthDate).WithErrorMessage(ErrorMessages.INVALID_BIRTH_DATE);
        }

    }
}
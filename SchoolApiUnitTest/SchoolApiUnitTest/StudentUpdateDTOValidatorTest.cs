using FluentValidation.TestHelper;
using SchoolAPI.Constants;
using SchoolAPI.DTO;
using SchoolAPI.Validators;

namespace SchoolAPI.Tests.Validators
{
    public class StudentUpdateValidatorTests
    {
        private readonly StudentUpdateValidator _validator;

        public StudentUpdateValidatorTests()
        {
            _validator = new StudentUpdateValidator();
        }

        [Fact]
        public void Should_NotHaveError_When_FirstNameIsNull()
        {
            var model = new StudentUpdateDTO { FirstName = null };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(student => student.FirstName);
        }

        [Fact]
        public void Should_HaveError_When_FirstNameIsTooShortOrTooLong()
        {
            var model = new StudentUpdateDTO { FirstName = "A" };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage(ErrorMessages.FIRST_NAME_MIN_LENGTH);

            model.FirstName = new string('A', 16);
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage(ErrorMessages.FIRST_NAME_MAX_LENGTH);
        }

        [Fact]
        public void Should_HaveError_When_LastNameIsTooShortOrTooLong()
        {
            var model = new StudentUpdateDTO { LastName = "B" };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.LastName).WithErrorMessage(ErrorMessages.LAST_NAME_MIN_LENGTH);;

            model.LastName = new string('B', 16);
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.LastName).WithErrorMessage(ErrorMessages.LAST_NAME_MAX_LENGTH);;
        }

        [Fact]
        public void Should_NotHaveError_When_EmailIsNull()
        {
            var model = new StudentUpdateDTO { Email = null };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(student => student.Email);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            var model = new StudentUpdateDTO { Email = "invalid-email" };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Email).WithErrorMessage(ErrorMessages.INVALID_EMAIL);
        }

        [Fact]
        public void Should_NotHaveError_When_PhoneIsNull()
        {
            var model = new StudentUpdateDTO { Phone = null };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(student => student.Phone);
        }

        [Fact]
        public void Should_HaveError_When_PhoneIsInvalid()
        {
            var model = new StudentUpdateDTO { Phone = "12345" };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.Phone).WithErrorMessage(ErrorMessages.INVALID_PHONE_NUMBER);
        }

        [Fact]
        public void Should_NotHaveError_When_BirthDateIsNull()
        {
            var model = new StudentUpdateDTO { BirthDate = null };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(student => student.BirthDate);
        }

        [Fact]
        public void Should_HaveError_When_BirthDateIsInvalid()
        {
            var model = new StudentUpdateDTO { BirthDate = DateTime.Now.AddYears(-5) };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.BirthDate).WithErrorMessage(ErrorMessages.INVALID_BIRTH_DATE);
        }
    }
}

using FluentValidation;
using SchoolAPI.Constants;
using SchoolAPI.DTO;

namespace SchoolAPI.Validators
{
    public class StudentPostDTOValidator : AbstractValidator<StudentPostDTO>
    {
        public StudentPostDTOValidator()
        {
            RuleFor(student => student.FirstName)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.FIRST_NAME_REQUIRED)
                .MinimumLength(2).WithMessage(ErrorMessages.FIRST_NAME_MIN_LENGTH)
                .MaximumLength(15).WithMessage(ErrorMessages.FIRST_NAME_MAX_LENGTH);

            RuleFor(student => student.LastName)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.LAST_NAME_REQUIRED)
                .MinimumLength(2).WithMessage(ErrorMessages.LAST_NAME_MIN_LENGTH)
                .MaximumLength(15).WithMessage(ErrorMessages.LAST_NAME_MAX_LENGTH);

            RuleFor(student => student.Email)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.EMAIL_REQUIRED)
                .EmailAddress().WithMessage(ErrorMessages.INVALID_EMAIL);

            RuleFor(student => student.Phone)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.PHONE_REQUIRED)
                .Matches("^[6-9]\\d{9}$").WithMessage(ErrorMessages.INVALID_PHONE_NUMBER);

            RuleFor(student => student.BirthDate)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.BIRTH_DATE_REQUIRED)
                .Must(BeAValidBirthDate).WithMessage(ErrorMessages.INVALID_BIRTH_DATE);

        }

        private bool BeAValidBirthDate(DateTime birthDate)
        {
            var minDate = DateTime.Now.AddYears(-10);
            return birthDate <= minDate;
        }
    }
}

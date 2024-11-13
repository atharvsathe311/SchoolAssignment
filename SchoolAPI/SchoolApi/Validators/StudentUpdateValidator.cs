using FluentValidation;
using CommonLibrary.Constants;
using SchoolAPI.DTO;

namespace SchoolAPI.Validators
{
    public class StudentUpdateValidator:AbstractValidator<StudentUpdateDTO>
    {
        public StudentUpdateValidator()
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(2).WithMessage(ErrorMessages.FIRST_NAME_MIN_LENGTH)
                .MaximumLength(15).WithMessage(ErrorMessages.FIRST_NAME_MAX_LENGTH)
                .When(s => !string.IsNullOrEmpty(s.FirstName));                

            RuleFor(student => student.LastName)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.LAST_NAME_REQUIRED)
                .MinimumLength(2).WithMessage(ErrorMessages.LAST_NAME_MIN_LENGTH)
                .MaximumLength(15).WithMessage(ErrorMessages.LAST_NAME_MAX_LENGTH)
                .When(s => !string.IsNullOrEmpty(s.LastName));

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage(ErrorMessages.INVALID_EMAIL)
                .When(s => !string.IsNullOrEmpty(s.Email));

            RuleFor(student => student.Phone)
                .Matches("^[6-9]\\d{9}$").WithMessage(ErrorMessages.INVALID_PHONE_NUMBER)
                .When(s => !string.IsNullOrEmpty(s.Phone));

            RuleFor(student => student.BirthDate)                
                .Must(BeAValidBirthDate).WithMessage(ErrorMessages.INVALID_BIRTH_DATE);

        }

        private bool BeAValidBirthDate(DateTime? birthDate)
        {
            if (birthDate.HasValue)
            {
                var minDate = DateTime.Now.AddYears(-10);
                return birthDate <= minDate;
            }
            return true;
        }
    }
}
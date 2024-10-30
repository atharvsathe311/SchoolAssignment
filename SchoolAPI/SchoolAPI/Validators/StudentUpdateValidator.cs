using FluentValidation;
using SchoolApi.Core.DTO;

namespace SchoolAPI.Validators
{
    public class StudentUpdateValidator:AbstractValidator<StudentUpdateDTO>
    {
        public StudentUpdateValidator()
        {
            RuleFor(student => student.FirstName)
                .MinimumLength(2).WithMessage("FirstName must be atleast 2 Characters")
                .MaximumLength(15).WithMessage("FirstName cannot exceed 15 Characters")
                .When(s => !string.IsNullOrEmpty(s.FirstName));                

            RuleFor(student => student.LastName).NotNull().NotEmpty()
                .MinimumLength(2).WithMessage("LastName must be atleast 2 Characters")
                .MaximumLength(15).WithMessage("LastName cannot exceed 15 Characters")
                .When(s => !string.IsNullOrEmpty(s.LastName));

            RuleFor(student => student.Email)
                .EmailAddress().WithMessage("Invalid Email")
                .When(s => !string.IsNullOrEmpty(s.Email));

            RuleFor(student => student.Phone)
                .Matches("^[6-9]\\d{9}$").WithMessage("Invalid Phone Number")
                .When(s =>!string.IsNullOrEmpty(s.Phone));

            RuleFor(student => student.BirthDate)                
            .Must(BeAValidBirthDate).WithMessage("Invalid Birthdate");

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
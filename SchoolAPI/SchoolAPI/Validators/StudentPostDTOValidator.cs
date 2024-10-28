using FluentValidation;
using SchoolApi.Core.DTO;

namespace SchoolAPI.Validators
{
    public class StudentPostDTOValidator : AbstractValidator<StudentPostDTO>
    {
        public StudentPostDTOValidator()
        {
            RuleFor(student => student.FirstName)
                .NotNull().NotEmpty().WithMessage("FirstName is Required")
                .MinimumLength(2).WithMessage("FirstName must be atleast 2 Characters")
                .MaximumLength(15).WithMessage("FirstName cannot exceed 15 Characters");

            RuleFor(student => student.LastName).NotNull().NotEmpty().WithMessage("LastName is Required")
                .MinimumLength(2).WithMessage("LastName must be atleast 2 Characters")
                .MaximumLength(15).WithMessage("LastName cannot exceed 15 Characters");

            RuleFor(student => student.Email).NotNull().NotEmpty().WithMessage("Email is Required")
                .EmailAddress().WithMessage("Invalid Email");

            RuleFor(student => student.Phone).NotNull().NotEmpty().WithMessage("Phone Number is Required")
                .Matches("^[6-9]\\d{9}$").WithMessage("Invalid Phone Number");

            RuleFor(student => student.BirthDate).NotNull().NotEmpty().WithMessage("BirthDate is Required").Must(BeAValidBirthDate).WithMessage("Invalid Birthdate");

        }

        private bool BeAValidBirthDate(DateTime birthDate)
        {
            var minDate = DateTime.Now.AddYears(-10);
            return birthDate <= minDate;
        }
    }
}

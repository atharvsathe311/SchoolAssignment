using FluentValidation;
using SchoolApi.DTO;
using SchoolAPI.Validators;

namespace SchoolApi.Validators
{
    public class NewStudentSagaValidator : AbstractValidator<NewSagaStudent>
    {
        public NewStudentSagaValidator()
        {
            RuleFor(student => student.Student)
                .NotNull().WithMessage("StudentPostDTO is required.")
                .SetValidator(new StudentPostDTOValidator());

            RuleFor(student => student.CourseIds)
                .Must(courseIds => courseIds.All(id => id > 0))
                .WithMessage("All CourseIds must be greater than 0.")
                .When(student => student.CourseIds != null);

        }


    }
}
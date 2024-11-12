using FluentValidation;
using UserAPI.Constants;
using UserAPI.DTO;

namespace UserAPI.Validators
{
    public class UserPostValidator: AbstractValidator<UserPostDTO>
    {        
        public UserPostValidator() 
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage(ErrorMessages.EMPTY_USERNAME)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$")
                .WithMessage(ErrorMessages.USERNAME_VALIDATION);

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(ErrorMessages.EMAIL_REQUIRED)
                .EmailAddress().WithMessage(ErrorMessages.INVALID_EMAIL);

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage(ErrorMessages.EMPTY_PASSWORD)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage(ErrorMessages.PASSWORD_VALIDATION);
        }
    }
}
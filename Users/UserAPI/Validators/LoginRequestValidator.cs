using CommonLibrary.Constants;
using FluentValidation;
using UserAPI.Core.GeneralModels;

namespace UserAPI.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator(){
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
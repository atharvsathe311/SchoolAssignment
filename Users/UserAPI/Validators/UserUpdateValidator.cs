using CommonLibrary.Constants;
using FluentValidation;
using UserAPI.DTO;

namespace UserAPI.Validators
{
    public class UserUpdateValidator: AbstractValidator<UserUpdateDTO>
    {        
        public UserUpdateValidator() 
        {
            RuleFor(user => user.Email)
                .EmailAddress().WithMessage(ErrorMessages.INVALID_EMAIL).When(s=> s.Email != null);

            RuleFor(user => user.Password)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage(ErrorMessages.PASSWORD_VALIDATION).When(s=> s.Password != null);
        }
    }
}
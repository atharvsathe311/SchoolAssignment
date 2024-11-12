using UserAPI.GeneralModels;

namespace UserAPI.Exceptions
{
    public class ValidationException:Exception
    {
        public ValidationErrors ValidationErrors { get; }   
        public ValidationException(ValidationErrors validationErrors) : base()
        {
            ValidationErrors = validationErrors;
        }
        
    }
}
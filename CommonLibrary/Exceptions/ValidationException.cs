using CommonLibrary.GeneralModels;

namespace CommonLibrary.Exceptions
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
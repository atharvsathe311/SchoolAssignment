using CommonLibrary.GeneralModels;

namespace CommonLibrary.Exceptions
{
    public class CustomException:Exception
    {
        public ErrorDetails ErrorDetails { get;}
        public CustomException(ErrorDetails errorDetails) : base()
        {
            ErrorDetails = errorDetails;
        }
    }
}
using SchoolApi.Core.GenearalModels;

namespace SchoolAPI.Exceptions
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
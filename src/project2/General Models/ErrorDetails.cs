namespace SchoolApi.Core.GenearalModels
{
    public class ErrorDetails
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode  {get; set;}
        public string ExceptionMessage {get; set;} = string.Empty;
    }
}
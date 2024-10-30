namespace SchoolApi.Core.Models
{
    public class ErrorDetails
    {
        public Guid TraceId { get; set; }
        public string? Message { get; set; }
        public int StatusCode  {get; set;}
        public string? Instance {get; set;}
        public string? ExceptionMessage {get; set;}
    }
}
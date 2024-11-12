namespace SchoolApi.Core.GenearalModels
{
    public class ApiLog
    {
        public required string Method {get;set;} 
        public required string Path {get;set;}
        public int StatusCode {get;set;}
        public DateTime Timestamp {get;set;}
    }
}
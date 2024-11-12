using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolApi.Core.GenearalModels;
using Serilog;

namespace SchoolApi.Filters
{
    public class APILoggingFilter : IActionFilter
    {
        private readonly Serilog.ILogger _logger;
 
        public APILoggingFilter()
        {
            _logger = Log.ForContext<APILoggingFilter>();
        }
 
        public void OnActionExecuting(ActionExecutingContext context)
        {
           
        }
 
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var logEntry = new ApiLog
            {
                Method = context.HttpContext.Request.Method,
                Path = context.HttpContext.Request.Path,
                StatusCode = context.HttpContext.Response.StatusCode,
                Timestamp = DateTime.Now
            };
 
            _logger.Information("{@ApiLogEntry}", logEntry);
 
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var returnedObject = objectResult.Value;
               
                _logger.Information(
                    "Returned Object: {@ReturnedObject}",
                    returnedObject
                );
            }
        }
    }
}
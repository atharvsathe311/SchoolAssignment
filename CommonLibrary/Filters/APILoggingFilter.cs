using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CommonLibrary.GeneralModels;
using Serilog;

namespace CommonLibrary.Filters
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
            if (context.HttpContext.Request.Method == HttpMethods.Post ||
                context.HttpContext.Request.Method == HttpMethods.Put ||
                context.HttpContext.Request.Method == HttpMethods.Patch)
            {
                context.HttpContext.Request.EnableBuffering();
                foreach (var arg in context.ActionArguments)
                {
                    _logger.Information("Request Data: {@Value}", arg.Value);
                }
                context.HttpContext.Request.Body.Position = 0;
            }
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
                _logger.Information("Returned Object: {@ReturnedObject}", returnedObject);
            }
        }
    }
}

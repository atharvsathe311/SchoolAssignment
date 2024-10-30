using System.Net;
using SchoolApi.Core.Models;

namespace SchoolAPI.GlobalException
{
    public class GlobalExceptionHandler:IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
 
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
 
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Guid.NewGuid();
            _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
 
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
 
            var errorDetails = new ErrorDetails()
            {
                TraceId = traceId,
                Message = "Internal Server Error!",
                StatusCode = context.Response.StatusCode,
                Instance = context.Request.Path,
                ExceptionMessage = exception.Message
            };
 
            return context.Response.WriteAsJsonAsync(errorDetails);
        }
    }
}
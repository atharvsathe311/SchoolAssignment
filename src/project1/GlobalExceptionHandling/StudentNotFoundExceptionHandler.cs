using Microsoft.AspNetCore.Diagnostics;
using SchoolApi.Core.GenearalModels;
using SchoolAPI.Constants;
using SchoolAPI.Exceptions;

namespace SchoolAPI.GlobalExceptionHandling
{
    internal sealed class NotFoundExceptionHandler:IExceptionHandler
    {
        
        private readonly ILogger<NotFoundExceptionHandler> _logger;

        public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {
            if (exception is not StudentNotFoundException notFoundException )
            {
                return false;
            }

            var traceId = Guid.NewGuid();
            _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
    

            var problemDetails = new ErrorDetails
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ErrorMessages.NOT_FOUND,
                ExceptionMessage = exception.Message
            };

            httpContext.Response.StatusCode = problemDetails.StatusCode;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }   
    }
}
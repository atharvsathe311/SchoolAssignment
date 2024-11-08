using Microsoft.AspNetCore.Diagnostics;
using SchoolApi.Core.GenearalModels;
using SchoolAPI.Constants;

namespace SchoolAPI.GlobalExceptionHandling
{
    internal sealed class AnyExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<AnyExceptionHandler> _logger;

        public AnyExceptionHandler(ILogger<AnyExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {

            var traceId = Guid.NewGuid();

            _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            var problemDetails = new ErrorDetails
            {
                Message = ErrorMessages.INTERNAL_SERVER_ERROR,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = problemDetails.StatusCode;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using SchoolApi.Core.GenearalModels;
using SchoolAPI.Constants;

namespace SchoolAPI.GlobalExceptionHandling
{
    internal sealed class GeneraliseExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GeneraliseExceptionHandler> _logger;

        public GeneraliseExceptionHandler(ILogger<GeneraliseExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not Exception generalException)
            {
                return false;
            }

            var traceId = Guid.NewGuid();

            _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            var problemDetails = new ErrorDetails
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ErrorMessages.UNKNOWN_ERROR,
                ExceptionMessage = generalException.Message
            };

            httpContext.Response.StatusCode = problemDetails.StatusCode;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;

        }
    }
}
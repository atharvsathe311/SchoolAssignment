using Microsoft.AspNetCore.Diagnostics;
using CommonLibrary.Constants;
using CommonLibrary.Exceptions;
using CommonLibrary.GeneralModels;

namespace CommonLibrary.GlobalExceptionHandling
{
    public sealed class GeneraliseExceptionHandler : IExceptionHandler
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

            if (exception is CustomException generalException)
            {

                var traceId = Guid.NewGuid();

                _logger.LogError($"TraceId: {traceId}, Exception: {generalException.Message}, StackTrace: {generalException.StackTrace}");

                var problemDetails = generalException.ErrorDetails;

                httpContext.Response.StatusCode = generalException.ErrorDetails.StatusCode;

                await httpContext.Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;
            }
            
            if(exception is ValidationException validationException)
            {

                var traceId = Guid.NewGuid();

                _logger.LogError($"TraceId: {traceId}, Exception: {validationException.Message}, StackTrace: {validationException.StackTrace}");

                var problemDetails = validationException.ValidationErrors;

                httpContext.Response.StatusCode = validationException.ValidationErrors.StatusCode;

                await httpContext.Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;

            }

            var traceIdGeneral = Guid.NewGuid();

            _logger.LogError($"TraceId: {traceIdGeneral}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            var problemDetailsGeneral = new ErrorDetails
            {
                Message = ErrorMessages.INTERNAL_SERVER_ERROR,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = problemDetailsGeneral.StatusCode;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetailsGeneral, cancellationToken);

            return true;
            
        }
    }
}
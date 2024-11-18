using Microsoft.AspNetCore.Diagnostics;
using CommonLibrary.Constants;
using CommonLibrary.Exceptions;
using CommonLibrary.GeneralModels;
using Serilog;
using CommonLibrary.Filters;

namespace CommonLibrary.GlobalExceptionHandling
{
    public sealed class GeneraliseExceptionHandler : IExceptionHandler
    {
        private readonly Serilog.ILogger _logger;

        public GeneraliseExceptionHandler()
        {
            _logger = Log.ForContext<APILoggingFilter>();
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Guid.NewGuid();

            if (exception is CustomException generalException)
            {
                _logger.Error($"TraceId: {traceId}, Exception: {generalException.Message}, StackTrace: {generalException.StackTrace}");

                var problemDetails = generalException.ErrorDetails;

                httpContext.Response.StatusCode = generalException.ErrorDetails.StatusCode;

                await httpContext.Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;
            }
            
            if(exception is ValidationException validationException)
            {
                _logger.Error($"TraceId: {traceId}, Exception: {validationException.Message}, StackTrace: {validationException.StackTrace}");

                var problemDetails = validationException.ValidationErrors;

                httpContext.Response.StatusCode = validationException.ValidationErrors.StatusCode;

                await httpContext.Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;

            }

            _logger.Error($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            var problemDetailsGeneral = new ErrorDetails
            {
                Message = ErrorMessages.INTERNAL_SERVER_ERROR,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = problemDetailsGeneral.StatusCode;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetailsGeneral, cancellationToken);

            _logger.Information("Returned Object: {@ReturnedObject}", problemDetailsGeneral);
            return true;
        }
    }
}
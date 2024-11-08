using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Org.BouncyCastle.Asn1.Cmp;
using SchoolAPI.Exceptions;

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

            if (exception is CustomException generalException)
            {

                var traceId = Guid.NewGuid();

                _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

                var problemDetails = generalException.ErrorDetails;

                httpContext.Response.StatusCode = generalException.ErrorDetails.StatusCode;

                await httpContext.Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;
            }
            return false;
        }
    }
}
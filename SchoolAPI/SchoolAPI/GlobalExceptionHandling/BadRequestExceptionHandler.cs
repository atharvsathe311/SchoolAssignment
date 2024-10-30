using Microsoft.AspNetCore.Diagnostics;
using SchoolApi.Core.GenearalModels;

namespace SchoolAPI.GlobalExceptionHandling
{internal sealed class BadRequestExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BadRequestExceptionHandler> _logger;

    public BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequestException)
        {
            return false;
        }

        var traceId = Guid.NewGuid();
        
        _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

        var problemDetails = new ErrorDetails
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = badRequestException.Message,
            ExceptionMessage = badRequestException.Message
        };

        httpContext.Response.StatusCode = problemDetails.StatusCode;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
}
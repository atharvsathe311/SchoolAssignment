// using FluentValidation;
// using Microsoft.AspNetCore.Diagnostics;
// using SchoolApi.Core.GenearalModels;
// using SchoolAPI.Constants;

// namespace SchoolAPI.GlobalExceptionHandling
// {
//     internal sealed class ValidationExceptionHandler:IExceptionHandler
//     {
        
//         private readonly ILogger<ValidationExceptionHandler> _logger;

//         public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
//         {
//             _logger = logger;
//         }

//         public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
//         {
//             if (exception is not ValidationException validationException )
//             {
//                 return false;
//             }

//             var traceId = Guid.NewGuid();
//             _logger.LogError($"TraceId: {traceId}, Exception: {validationException.Message}, StackTrace: {validationException.StackTrace}");
    

//             var problemDetails = new ErrorDetails
//             {
//                 StatusCode = StatusCodes.Status400BadRequest,
//                 Message = ErrorMessages.ValidationError,
//                 ExceptionMessage = exception.Message
//             };

//             httpContext.Response.StatusCode = problemDetails.StatusCode;

//             await httpContext.Response
//                 .WriteAsJsonAsync(problemDetails, cancellationToken);

//             return true;
//         }   
//     }
// }


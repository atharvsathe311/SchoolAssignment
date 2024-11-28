using Microsoft.AspNetCore.Mvc.Filters;
using CommonLibrary.GeneralModels;
using CommonLibrary.Exceptions;
using CommonLibrary.Constants;
using Serilog;
using Newtonsoft.Json;

namespace CommonLibrary.Filters
{
    public class ValidationFilter : IActionFilter
    {
        private readonly Serilog.ILogger _logger;

        public ValidationFilter()
        {
            _logger = Log.ForContext<APILoggingFilter>();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var ExceptionErrors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(err => err.ErrorMessage).ToList()
                        );

                var errorDetails = new ValidationErrors
                {
                    Message = ErrorMessages.ValidationError,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = ExceptionErrors
                };

                _logger.Information(JsonConvert.SerializeObject(ExceptionErrors));

                throw new ValidationException(errorDetails);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc.Filters;
using UserAPI.Constants;
using UserAPI.Exceptions;
using UserAPI.GeneralModels;


namespace UserAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var ExceptionErrors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToList()
                        );
                         
                var errorDetails = new ValidationErrors
                {
                    Message = ErrorMessages.ValidationError,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = ExceptionErrors
                };
                
                throw new ValidationException(errorDetails);
            }
        }
    }
}
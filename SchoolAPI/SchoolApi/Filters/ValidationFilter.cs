using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolApi.Core.GenearalModels;
using SchoolApi.Exceptions;
using SchoolAPI.Constants;
using SchoolAPI.Exceptions;

namespace SchoolAPI.Filters
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
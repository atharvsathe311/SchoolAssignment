using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolApi.Core.GenearalModels;
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
                var ExceptionErrors = string.Join(",", 
                        context.ModelState
                            .Where(e => e.Value.Errors.Count > 0)
                            .SelectMany(kvp => kvp.Value.Errors.Select(err => err.ErrorMessage))
                    );
                         
                var errorDetails = new ErrorDetails
                {
                    Message = ExceptionErrors,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                
                throw new CustomException(errorDetails);
            }
        }
    }
}
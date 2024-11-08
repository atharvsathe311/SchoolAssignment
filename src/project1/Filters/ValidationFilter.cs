using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolAPI.Constants;

namespace SchoolAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            if (!context.ModelState.IsValid)
            {
                 var errorDetails = new 
                    {
                        Message = ErrorMessages.ValidationError,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ExceptionErrors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                        )
                    };
                context.Result = new BadRequestObjectResult(errorDetails);
            }
        }
    }
}
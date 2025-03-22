using BankDirectoryApi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BankDirectoryApi.API.Filters
{
    public class CustomValidationFailureFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState != null && !context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Any() == true)
                    .Select(x => new ApiError
                    {
                        Field = x.Key,
                        Message = x.Value?.Errors.First().ErrorMessage ?? "Unknown error"
                    })
                    .ToList();

                context.Result = new BadRequestObjectResult(new 
                    ApiResponse<object>(errors, null, (int)HttpStatusCode.BadRequest));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}

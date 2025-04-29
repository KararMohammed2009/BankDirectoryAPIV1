using BankDirectoryApi.API.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankDirectoryApi.API.Extensions
{
    public static class ValidationExtensions
    {
        public static List<ApiError> ToApiErrors(this List<ValidationFailure> errors)
        {
            return errors.Select(error => new ApiError
            {
                Code = error.ErrorCode,
                Message = error.ErrorMessage,
                Field = error.PropertyName
            }).ToList();     
        }
        public static IActionResult ToBadRequest(this ValidationResult validationResult)
        {
            return new BadRequestObjectResult(new ApiResponse<object>(
                validationResult.Errors.ToApiErrors(), null, (int)HttpStatusCode.BadRequest));
        }
    }
}

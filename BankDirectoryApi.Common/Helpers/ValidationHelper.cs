using BankDirectoryApi.Common.Errors;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Helpers
{
    /// <summary>
    /// Helper class for validation
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates if a string is null or whitespace.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <returns> Result indicating success or failure.</returns>
        public static Result ValidateNullOrWhiteSpaceString(string parameter, string? parameterName=null)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = nameof(parameter);
            }
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return Result.Fail(new Error($"{parameterName} is required")
                    .WithMetadata("ErrorCode", CommonErrors.MissingRequiredField));
            }
            return Result.Ok();
        }
        /// <summary>
        /// Validates if a model is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="modelName"></param>
        /// <returns> Result indicating success or failure.</returns>
        public static Result ValidateNullModel<T>(T model, string? modelName = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(modelName))
            {
                modelName = typeof(T).Name;
            }
            if (model == null)
            {
                return Result.Fail(new Error($"{modelName} is required")
                    .WithMetadata("ErrorCode", CommonErrors.MissingRequiredField));
            }
            return Result.Ok();
        }
    }
}

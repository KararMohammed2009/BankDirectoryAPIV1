using BankDirectoryApi.Common.Errors;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Helpers
{
    public static class ValidationHelper
    {
        public static Result ValidateNullOrWhiteSpaceString(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return Result.Fail(new Error($"{parameterName} is required")
                    .WithMetadata("ErrorCode", CommonErrors.MissingRequiredField));
            }
            return Result.Ok();
        }

        public static Result ValidateNullModel<T>(T model, string modelName) where T : class
        {
            if (model == null)
            {
                return Result.Fail(new Error($"{modelName} is required")
                    .WithMetadata("ErrorCode", CommonErrors.MissingRequiredField));
            }
            return Result.Ok();
        }
    }
}

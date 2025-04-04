using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Extensions
{
    public static class FluentResultExtentions
    {
        public static Result IncludeIdentityErrors(this Result result, IdentityResult identityResult)
        {
            if (identityResult == null)
            {
                return result;
            }

            foreach (var error in identityResult.Errors)
            {
                result.WithError(error.Description);
            }

            return result;
        }
        public static Result IncludeIdentityErrors(this Result result, SignInResult signInResult)
        {
            if (signInResult == null)
            {
                return result;
            }
            if(signInResult.IsNotAllowed)
            {
                result.WithError("User is not allowed to sign in.");
            }
            if (signInResult.IsLockedOut)
            {
                result.WithError("User is locked out.");
            }
            return result;
        }
    }
}

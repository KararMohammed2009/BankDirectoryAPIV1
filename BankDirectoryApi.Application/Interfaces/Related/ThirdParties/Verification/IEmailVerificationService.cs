using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification
{
    /// <summary>
    /// Interface for email verification services.
    /// </summary>
    public interface IEmailVerificationService
    {
        /// <summary>
        /// Sends a verification code to the specified email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The result of the verification code sending operation.</returns>
        Task<Result> SendVerificationCodeAsync(string email);
        /// <summary>
        /// Verifies the provided code for the specified email address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>The result of the verification operation.</returns>
        Task<Result<bool>> VerifyCodeAsync(string email, string code);
    }
}

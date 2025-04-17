using FluentResults;

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
        /// <returns>The result of the verification operation with a boolean indicating success.</returns>
        Task<Result<bool>> VerifyCodeAsync(string email, string code);
    }
}

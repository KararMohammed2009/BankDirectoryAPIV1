using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification
{
    /// <summary>
    /// Interface for verification services.
    /// </summary>
    public interface ISmsVerificationService
    {
        /// <summary>
        /// Sends a verification code to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>The result of the verification code sending operation.</returns>
        Task<Result> SendVerificationCodeAsync(string phoneNumber);
        /// <summary>
        /// Verifies the provided code for the specified phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns>The result of the verification operation with a boolean indicating success.</returns>
        Task<Result<bool>> VerifyCodeAsync(string phoneNumber, string code);
    }
}

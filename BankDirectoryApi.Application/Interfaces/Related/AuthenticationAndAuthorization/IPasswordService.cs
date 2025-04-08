using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user passwords
    /// </summary>
    public interface IPasswordService
    {

        /// <summary>
        /// Check if the password is correct and sign in the user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> CheckPasswordSignInAsync(LoginUserDTO model, string password, bool lockoutOnFailure);
        /// <summary>
        /// Change the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        /// <summary>
        /// Reset the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> ResetPasswordAsync(string userId, string token, string newPassword);
        /// <summary>
        /// Generate a password reset token for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> GeneratePasswordResetTokenAsync(string userId);
        /// <summary>
        /// Send a password reset email to a user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The value of the password reset token</returns>
        Task<Result<string>> ForgotPasswordAsync(string email);
    }
}

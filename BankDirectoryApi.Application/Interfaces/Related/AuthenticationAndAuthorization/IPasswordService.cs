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
        Task<Result<string>> ChangePasswordAsync(string userId, string currentPassword,
            string newPassword, ClientInfo clientInfo);
        /// <summary>
        /// Reset the password of a user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="newPassword"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> ResetPasswordAsync(string email, string code, string newPassword);
        /// <summary>
        /// Send a password reset token to user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The value of the password reset token whether it was sent or not</returns>
        Task<Result> ForgotPasswordAsync(string email);
    }
}

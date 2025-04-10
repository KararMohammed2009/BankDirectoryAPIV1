
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.UserManagement
{
    /// <summary>
    /// Service interface for user management operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users based on the provided filter criteria.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of users that match the filter criteria.</returns>
        Task<Result<List<UserDTO>>> GetAllUsersAsync(UserFilterDTO model,CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The user with the specified ID.</returns>
        Task<Result<UserDTO>> GetUserByIdAsync(string userId);
        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The user with the specified email address.</returns>
        Task<Result<UserDTO>> GetUserByEmailAsync(string email);
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The user with the specified username.</returns>
        Task<Result<UserDTO>> GetUserByUserNameAsync(string userName);
        /// <summary>
        /// Checks if a user exists by their email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True if the user exists, false otherwise.</returns>
        Task<Result<bool>> UserExistsByEmailAsync(string email);
        /// <summary>
        /// Updates a user with the provided information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The updated user.</returns>
        Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model);
        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of user id.</returns>
        Task<Result<string>> DeleteUserAsync(string userId);
        /// <summary>
        /// Creates a new user with the provided information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The created user.</returns>
        Task<Result<UserDTO>> CreateUserAsync(RegisterUserDTO model);
        /// <summary>
        /// Confirms a user's email address using the provided token.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns>The value of the user id.</returns>
        Task<Result<string>> ConfirmEmailAsync(string email, string token);
        /// <summary>
        /// Checks if a user's email is confirmed.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if the email is confirmed, false otherwise.</returns>
        Task<Result<bool>> IsEmailConfirmedAsync(UserDTO model);
        /// <summary>
        /// Generates a token for email confirmation.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The value of email confirmation token.</returns>
        Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email);
        /// <summary>
        /// External login for a user using the provided information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="externalAccessToken"></param>
        /// <param name="providerName"></param>
        /// <returns>The value of the user id.</returns>
        Task<Result<string>> AddLoginAsync(string id,string email, string name,string externalAccessToken,string providerName);
        /// <summary>
        /// Set Two-Factor Authentication enabled or disabled for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns>The value of the user id.</returns>
        Task<Result<string>> SetTwoFactorAuthenticationAsync(string userId, bool enabled);
        /// <summary>
        /// Get the claims of a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The claims of the user .</returns>
        Task<Result<Dictionary<string, string>>> GetUserCalimsAsync(string userId);

    }
}

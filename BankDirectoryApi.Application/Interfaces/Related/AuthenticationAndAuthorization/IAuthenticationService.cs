
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// This interface defines the contract for authentication services.
    /// </summary>
    public interface IAuthenticationService 
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the registration process, including authentication tokens.</returns>
        Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
        /// <summary>
        /// Registers a new user in the system by an admin.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the registration process, includeing user details.</returns>
        Task<Result<UserDTO>> RegisterByAdminAsync(RegisterUserByAdminDTO model);
        /// <summary>
        /// Logs in a user to the system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the login process, including authentication tokens.</returns>
        Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
        /// <summary>
        /// Logs in a user using an external authentication provider.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="providerName"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the external login process, including authentication tokens.</returns>
        Task<Result<AuthDTO>> ExternalLoginAsync(string code,string providerName, ClientInfo clientInfo);
        /// <summary>
        /// Logs out a user from the system.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The value of userId.</returns>
        Task<Result<string>> LogoutAsync(string userId, string sessionId, ClientInfo clientInfo);
        /// <summary>
        /// Generates a new access token using a refresh token.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the token generation process, including the new access token and refresh token.</returns>
        Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);
        /// <summary>
        /// Validates the provided access token.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>True if the access token is valid; otherwise, false.</returns>
        Task<Result<bool>> ValidateAccessTokenAsync(string accessToken);

    }
}

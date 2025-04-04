
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IAuthenticationService 
    {

         Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
         Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
         Task<Result<AuthDTO>> ExternalLoginAsync(string code,string providerName, ClientInfo clientInfo);
         Task<Result<string>> LogoutAsync(string userId, string sessionId, ClientInfo clientInfo);
         Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);
         Task<Result<bool>> ValidateAccessTokenAsync(string accessToken);

    }
}

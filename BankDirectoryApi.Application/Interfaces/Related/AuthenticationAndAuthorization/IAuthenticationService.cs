using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IAuthenticationService 
    {

         Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
         Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
         Task<Result<AuthDTO>> ExternalLoginAsync(string code, ClientInfo clientInfo);
         Task<Result<bool>> LogoutAsync(string userId, string sessionId, ClientInfo clientInfo);
         Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);
        Task <Result<bool>> ValidateAccessToken(string accessToken);
    }
}

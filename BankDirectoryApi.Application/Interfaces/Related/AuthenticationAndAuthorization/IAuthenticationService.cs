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

         Task<AuthDTO> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
         Task<AuthDTO> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
         Task<AuthDTO> ExternalLoginAsync(string code, ClientInfo clientInfo);
         Task<bool> LogoutAsync(string userId, string sessionId, ClientInfo clientInfo);
         Task<AuthDTO> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);
        Task <bool> ValidateAccessToken(string accessToken);
    }
}

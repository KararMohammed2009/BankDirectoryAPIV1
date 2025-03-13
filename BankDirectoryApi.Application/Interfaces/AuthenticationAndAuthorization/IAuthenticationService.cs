using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.UserManagement;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.AuthenticationAndAuthorization
{
    public interface IAuthenticationService<T> where T : class
    {
        public Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> ExternalLoginAsync(string code, ClientInfo clientInfo);
        public Task<Result<(string? accessToken, string? refreshToken)>> GenerateAndStoreTokensAsync(IdentityUser user, ClientInfo clientInfo);
        public Task<Result<bool>> LogoutAsync(string userId, string refreshToken, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);

    }
}

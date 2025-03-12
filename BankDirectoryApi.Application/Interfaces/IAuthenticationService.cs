using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> ExternalLoginAsync(string code, ClientInfo clientInfo);
        public Task<Result<(string? accessToken, string? refreshToken)>> GenerateAndStoreTokensAsync(IdentityUser user, ClientInfo clientInfo);
        public Task<Result<bool>> LogoutAsync(string userId, string refreshToken, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId, string refreshToken, ClientInfo clientInfo);

    }
}

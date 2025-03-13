using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.DTOs.UserManagement;
using BankDirectoryApi.Application.Interfaces.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.AuthenticationAndAuthorization
{
    public class AuthenticationService:IAuthenticationService
    {
        public async Task<(string? accessToken, string? refreshToken)> GenerateAndStoreTokensAsync(
        IdentityUser user
       , ClientInfo clientInfo)
        {
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);

            var refreshTokenResult = await GenerateRefreshTokenAsync(user, clientInfo);
            if (refreshTokenResult.refreshTokenEntity == null || string.IsNullOrEmpty(refreshTokenResult.refreshToken)) return (null, null);
            await _refreshTokenRepository.AddAsync(refreshTokenResult.refreshTokenEntity);

            return (accessToken, refreshTokenResult.refreshToken);
        }
        private async Task<(RefreshToken? refreshTokenEntity, string? refreshToken)> GenerateRefreshTokenAsync(
            IdentityUser user
           , ClientInfo clientInfo)
        {

            var refreshTokenPair = await _jwtService.GenerateRefreshTokenAsync(user);
            if (string.IsNullOrEmpty(refreshTokenPair.RefreshToken)
                || string.IsNullOrEmpty(refreshTokenPair.HashedRefreshToken)) return (null, null);

            var refreshTokenLifetimeDays = _configuration["JwtSettings:RefreshTokenLifetimeDays"];
            if (string.IsNullOrEmpty(refreshTokenLifetimeDays)) throw new InvalidOperationException(
                "Cannot read refreshTokenLifetimeDays from appsetting.json >>> JwtSettings:RefreshTokenLifetimeDays");
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenPair.HashedRefreshToken,
                UserId = user.Id,
                ExpirationDate = _dateTimeProvider.UtcNow.AddDays(double.Parse(refreshTokenLifetimeDays)),
                CreationDate = _dateTimeProvider.UtcNow,
                IsRevoked = false,
                IsUsed = false,
                SessionId = _jwtService.GenerateNewSessionIdAsync(),
                UserAgent = clientInfo.UserAgent,
                CreatedByIp = clientInfo.IpAddress,
            };


            return (refreshTokenEntity, refreshTokenPair.RefreshToken);
        }
        public async Task<AuthDTO?> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo)
        {
            var user = _mapper.Map<RegisterUserDTO, IdentityUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return null;
            var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
            return new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            };
        }
        public async Task<AuthDTO?> LoginAsync(LoginUserDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return null;
            var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
            return new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            };
        }
        public async Task<AuthDTO?> GenerateAccessTokenFromRefreshTokenAsync(string userId,
           string refreshToken, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var refreshTokenEntity = await GenerateRefreshTokenAsync(user, clientInfo);
            if (refreshTokenEntity.refreshTokenEntity == null) return null;
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
            if (string.IsNullOrEmpty(accessToken)) return null;
            var res = await _refreshTokenRepository.RotateRefreshTokenAsync(
                _hashService.GetHash(refreshToken), refreshTokenEntity.refreshTokenEntity);

            return new AuthDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.refreshToken,
            };
        }
        public async Task<bool?> Logout(string userId, string SessionId, ClientInfo clientInfo)//Token Blacklisting
        {

            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(
                userId, SessionId, clientInfo.IpAddress);
            return true;
        }
        public async Task<AuthDTO?> ExternalLoginAsync(string code, ClientInfo clientInfo)
        {
            var externalLoginInfo = await _externalAuthProvider.ManageExternalLogin(code, clientInfo);
            if (!externalLoginInfo.Success) return null;
            return externalLoginInfo.Response;
        }
    }
}

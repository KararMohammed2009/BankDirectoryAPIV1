using AutoMapper;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Infrastructure.Data;
using Microsoft.SqlServer.Server;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Application.DTOs.Auth;
using YourProject.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace YourProject.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IdentityService _identityService;  // A service to handle JWT and refresh tokens
        private readonly IServiceProvider _serviceProvider;

        public UserService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           ApplicationDbContext dbContext,
                           IdentityService identityService,
                           IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _identityService = identityService;
            _serviceProvider = serviceProvider;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<AuthResponseDTO> GenerateTokensAsync(User user)
        {
            // Generate JWT and Refresh Token
            var jwtToken = await _identityService.GenerateJwtToken(user);
            var refreshToken = _identityService.GenerateJwtRefreshToken(user);

            // Store refresh token in the database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                CreationDate = DateTime.UtcNow,
                IsRevoked = false,
                
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _userManager.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            return await GenerateTokensAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found.");

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> LogoutAsync(string refreshToken)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (refreshTokenEntity != null)
            {
                _dbContext.RefreshTokens.Remove(refreshTokenEntity);
                await _dbContext.SaveChangesAsync();
            }

            return IdentityResult.Success;  // Return success on logout
        }

        public async Task<AuthResponseDTO> ExternalLoginAsync(string provider, string accessToken)
        {
            var externalAuthProvider = _serviceProvider.GetServices<IExternalAuthProvider>()
                                                   .FirstOrDefault(p => p.GetType().Name.StartsWith(provider, StringComparison.OrdinalIgnoreCase));

            if (externalAuthProvider == null)
            {
                throw new InvalidOperationException("External provider not supported.");
            }
            var result = (await externalAuthProvider.ValidateAndGetUserAsync(accessToken));
            if (!result.Success || result.User == null)
            {
                throw new InvalidOperationException($"External login failed ");
            }
            // Add login information (link external account to local account);
            var externalLoginInfo = new ExternalLoginInfo(
           user: result.User,
           loginProvider: provider,
           providerKey: result.User.ProviderKey, // Assuming you get this from the external provider result
           displayName: result.User.DisplayName // This could be the name or email of the user
       );
            var loginResult = await _userManager.AddLoginAsync(result.User , externalLoginInfo);

            if (!loginResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to add external login.");
            }


        }
    }

}

using AutoMapper;
using Azure.Core;
using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Communications;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.UserManagement
{
    public class UserService : IUserService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IExternalAuthProvider _externalAuthProvider;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHashService _hashService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        public UserService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IExternalAuthProvider externalAuthProvider,
            IJwtService jwtService, IMapper mapper
            , IRefreshTokenRepository refreshTokenRepository, IHashService hashService,
            IConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _externalAuthProvider = externalAuthProvider;
            _jwtService = jwtService;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
            _hashService = hashService;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }
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
        public async Task<UserDTO?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var userDTO = _mapper.Map<IdentityUser, UserDTO>(user);
            return userDTO;
        }
        public async Task<IEnumerable<UserDTO>?> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null) return null;

            var usersDTOs = _mapper.Map<IEnumerable<IdentityUser>, IEnumerable<UserDTO>>(users);
            return usersDTOs;
        }
        public async Task<bool?> UpdateUserAsync(UpdateUserDTO model)
        {
            var user = _mapper.Map<UpdateUserDTO, IdentityUser>(model);
            if (string.IsNullOrEmpty(user.Id)) return false;
            var oldUser = await _userManager.FindByIdAsync(user.Id);
            if (user == null) return false;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<bool?> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
        public async Task<bool?> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: Send token via email (implement email service)
            Console.WriteLine($"Password reset token for {model.Email}: {token}");

            return true;
        }
        public async Task<AuthDTO?> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded) return null;
            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id, clientInfo.IpAddress);
            var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
            return new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            };
        }
        public async Task<AuthDTO?> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded) return null;
            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id, clientInfo.IpAddress);
            var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
            return new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            };
        }

       
        public async Task<bool?> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return null;
            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
            return true;
        }
        public async Task<IEnumerable<string>?> DeleteUsersAsync(IEnumerable<string> userIds)
        {
            if (userIds == null) return null;
            var succeededDeletedUsers = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) continue;
                await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
                succeededDeletedUsers.Add(userId);
            }
            return succeededDeletedUsers;
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

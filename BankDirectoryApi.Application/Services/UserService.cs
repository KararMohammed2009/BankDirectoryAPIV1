using AutoMapper;
using Azure.Core;
using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IExternalAuthProvider _externalAuthProvider;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHashService _hashService;
        public UserService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager , 
            IExternalAuthProvider externalAuthProvider,
            IJwtService jwtService,IRefreshTokenService refreshTokenService,IMapper mapper
            ,IRefreshTokenRepository refreshTokenRepository,IHashService hashService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _externalAuthProvider = externalAuthProvider;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
            _hashService = hashService;
        }
        public async Task<AuthDTO?> RegisterAsync(RegisterUserDTO model)
        {
            var user = _mapper.Map<RegisterUserDTO, IdentityUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return null;

            return new AuthDTO
            {
                Token = await _jwtService.GenerateAccessTokenAsync(user),
                RefreshToken = await _jwtService.GenerateRefreshTokenAsync(user),
            };
        }
        public async Task<AuthDTO?> LoginAsync(LoginUserDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return null;

            return new AuthDTO
            {
                Token = await _jwtService.GenerateAccessTokenAsync(user),
                RefreshToken = await _jwtService.GenerateRefreshTokenAsync(user),
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
        public async Task<AuthDTO?> ResetPasswordAsync(ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            { 
                await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
                return new AuthDTO
                {
                    Token = await _jwtService.GenerateAccessTokenAsync(user),
                    RefreshToken = await _jwtService.GenerateRefreshTokenAsync(user),
                };

            }
                return null;
        }
        public async Task<AuthDTO?> ChangePasswordAsync(ChangePasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
                return new AuthDTO
                {
                    Token = await _jwtService.GenerateAccessTokenAsync(user),
                    RefreshToken = await _jwtService.GenerateRefreshTokenAsync(user),
                };

            }
            return null;
        }

        public async Task<bool?> ConfirmEmailAsync(EmailConfirmationDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            return result.Succeeded;
        }

        public async Task<bool?> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // TODO: Send token via email (implement email service)
            Console.WriteLine($"Email confirmation token for {email}: {token}");

            return true;
        }
        public async Task<bool?> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
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
                if (result.Succeeded) succeededDeletedUsers.Add(userId);
            }
            return succeededDeletedUsers;
        }
        public async Task<AuthDTO?> GenerateAccessTokenFromRefreshTokenAsync(string refreshToken)
        {
            
            bool? isValidRefreshToken = await _jwtService.ValidateTokenAsync(refreshToken);
            if (isValidRefreshToken == false) return null;
        
            var hashedRefreshToken = _hashService.GetHash(refreshToken);
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAndInsureValidityAsync(hashedRefreshToken);
            if (storedRefreshToken == null) return null;
            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            if (user == null) return null;

            var useStatus = await _refreshTokenRepository.UseRefreshTokenAsync(hashedRefreshToken);
            if (useStatus == false) return null;
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
            var rotatedRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user);
            return new AuthDTO
            {
                Token = accessToken,
                RefreshToken = rotatedRefreshToken,
            };
        }
        public async Task<bool?> Logout(string refreshToken) //Token Blacklisting
        {
            bool? isValidRefreshToken = await _jwtService.ValidateTokenAsync(refreshToken);
            if (isValidRefreshToken == false) return null;
            return await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);

        }
        public async Task<AuthDTO?> ExternalLoginAsync(string code)
        {
            var externalLoginInfo = await _externalAuthProvider.ManageExternalLogin(code);
            if (!externalLoginInfo.Success) return null;
            return externalLoginInfo.Response;
        }
    }
}

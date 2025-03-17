
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class PasswordService:IPasswordService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthenticationService _authenticationService;
        public PasswordService(
            IRefreshTokenRepository refreshTokenRepository,
            IAuthenticationService authenticationService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            
            _refreshTokenRepository = refreshTokenRepository;
            _authenticationService = authenticationService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            try
            {
                if (userId == null) throw new Exception("UserId is required");
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
                var result = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                if (result == null) throw new Exception("Password reset token generation failed by UserManager<IdentityUser>");
                return result;
            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Generate Password Reset Token failed", ex);
            }
        }
        public async Task<bool> CheckPasswordSignInAsync(string email, string password, bool lockoutOnFailure)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null) throw new Exception($"Cannot find user by email({email}) by UserManager<IdentityUser>");
                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure);
                return result.Succeeded;

            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Check Password Sign In failed", ex);
            }
        }
        public async Task<bool> ChangePasswordAsync(UserDTO user, string currentPassword, string newPassword)
        {
            try
            {
                if (user == null) throw new Exception("User is required");
                if (string.IsNullOrEmpty(currentPassword)) throw new Exception("Current password is required");
                if (string.IsNullOrEmpty(newPassword)) throw new Exception("New password is required");
                var identityUser = _mapper.Map<IdentityUser>(user);
                var result = await _userManager.ChangePasswordAsync(identityUser, currentPassword, newPassword);
                if (!result.Succeeded) throw new Exception("Change Password failed by UserManager<IdentityUser>");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Change Password failed", ex);
            }
        }
        public async Task<bool> ResetPasswordAsync(UserDTO user, string token, string newPassword)
        {
            try
            {
                if (user == null) throw new Exception("User is required");
                if (string.IsNullOrEmpty(token)) throw new Exception("Token is required");
                if (string.IsNullOrEmpty(newPassword)) throw new Exception("New password is required");

                var identityUser = _mapper.Map<IdentityUser>(user);
                var result = await _userManager.ResetPasswordAsync(identityUser, token, newPassword);
                if (!result.Succeeded) throw new Exception("Reset Password failed by UserManager<IdentityUser>");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Reset Password failed", ex);
            }
        }
        public async Task<string> ForgotPasswordAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null) throw new Exception($"Cannot find user by email({email}) by UserManager<IdentityUser>");
                var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                if (token == null) throw new Exception("Token generation failed by UserManager<IdentityUser>");

                return token;

            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Forgot Password failed", ex);

            }
        }























        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            try
            {
                var token = await _userService.GeneratePasswordResetTokenAsync(model.Email);
                // TODO: Send token via email (implement email service)


            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Forgot password failed", ex);
            } 
        }
        public async Task<AuthDTO> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return AuthDTO>.FailureResult(
                new List<Error> { new Error { Message = "Email Not Found", Severity = Severity.Error, Code = "EmailNotFound" } });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded) return AuthDTO>.FailureResult(
                result.Errors.Select(e => new Error { Message = e.Description, Severity = Severity.Error, Code = e.Code }).ToList());

            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id, clientInfo.IpAddress);
            var tokens = await _authenticationService.GenerateAndStoreTokensAsync(user, clientInfo);
            if(tokens == null) return AuthDTO>.FailureResult(
                new List<Error> { new Error { Message = "Failed to generate tokens", Severity = Severity.Error, Code = "TokenGenerationFailed" } });
         
            return AuthDTO>.SuccessResult(new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            });
        }
        public async Task<AuthDTO> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded) return null;
            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id, clientInfo.IpAddress);
            var tokens = await _authenticationService.GenerateAndStoreTokensAsync(user, clientInfo);
            return new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            };
        }
    }
}

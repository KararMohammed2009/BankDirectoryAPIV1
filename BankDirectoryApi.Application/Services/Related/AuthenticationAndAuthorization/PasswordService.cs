using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
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
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenRepository _authenticationService;
        public PasswordService(UserManager<IdentityUser> userManager, 
            IRefreshTokenRepository refreshTokenRepository,IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _authenticationService = authenticationService;
        }
        public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Email Not Found", Severity = Severity.Error, Code = "EmailNotFound" } });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: Send token via email (implement email service)

            return Result<bool>.SuccessResult(true);
        }
        public async Task<Result<AuthDTO>> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Result<AuthDTO>.FailureResult(
                new List<Error> { new Error { Message = "Email Not Found", Severity = Severity.Error, Code = "EmailNotFound" } });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded) return Result<AuthDTO>.FailureResult(
                result.Errors.Select(e => new Error { Message = e.Description, Severity = Severity.Error, Code = e.Code }).ToList());

            await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id, clientInfo.IpAddress);
            var tokens = await _authenticationService.GenerateAndStoreTokensAsync(user, clientInfo);
            if(tokens == null) return Result<AuthDTO>.FailureResult(
                new List<Error> { new Error { Message = "Failed to generate tokens", Severity = Severity.Error, Code = "TokenGenerationFailed" } });
         
            return Result<AuthDTO>.SuccessResult(new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            });
        }
        public async Task<Result<AuthDTO>> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo)
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

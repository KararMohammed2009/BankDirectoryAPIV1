
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Infrastructure;
using BankDirectoryApi.Domain.Entities.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user passwords
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<PasswordService> _logger;
        private readonly IRefreshTokenService _refreshTokenService;
        public PasswordService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            ,ILogger<PasswordService> logger,
IRefreshTokenService refreshTokenService)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Generate a password reset token for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of generated token</returns>
        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userId)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId,"userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();


            var user = await IdentityExceptionHelper.Execute(()=>
                _userManager.FindByIdAsync(userId),_logger);
                if (user == null)
                return Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.GeneratePasswordResetTokenAsync(user),_logger);
            if (string.IsNullOrWhiteSpace(result))
            {
                _logger.LogError($"Token generation failed by UserManager<ApplicationUser> for userId: {userId}");
                return Result.Fail(new Error("Token generation failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }

            return Result.Ok(result);
         
        }
        /// <summary>
        /// Check if the password is correct and sign in the user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns>The value of the user id</returns>
        public async Task<Result<string>> CheckPasswordSignInAsync(LoginUserDTO model, string password, bool lockoutOnFailure)
        {
            
                ApplicationUser? user;

            var validationResult = ValidationHelper.ValidateNullModel(model, "model");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(password, "password");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
          
            if (!string.IsNullOrEmpty(model.Email))
            {
                user = await IdentityExceptionHelper.Execute(() =>
                _userManager.FindByEmailAsync(model.Email), _logger);
                if (user == null)
                    return Result.Fail(new Error($"Cannot find user by email({model.Email}) by UserManager<ApplicationUser>")
                        .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
            else if (!string.IsNullOrEmpty(model.UserName))
                {
                    user = await IdentityExceptionHelper.Execute(() => 
                    _userManager.FindByNameAsync(model.UserName), _logger);
                    if (user == null)
                    return Result.Fail(new Error($"Cannot find user by Username({model.UserName}) by UserManager<ApplicationUser>")
                      .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
                else if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    user = await IdentityExceptionHelper.Execute(() => 
                    _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber),_logger);
                    if (user == null) 
                    return  Result.Fail(new Error($"Cannot find user by PhoneNumber({model.PhoneNumber}) by UserManager<ApplicationUser>")
                        .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
                else
                {
                   return Result.Fail(new Error("Either Email, UserName or PhoneNumber is required")
                        .WithMetadata("ErrorCode", CommonErrors.MissingRequiredField));
            }

                var result = await IdentityExceptionHelper.Execute(() => 
                _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure),_logger);
            if (!result.Succeeded)
            {
               return Result.Fail(new Error("Check Password SignIn failed by SignInManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnauthorizedAccess)).IncludeIdentityErrors(result);
            }
            return Result.Ok(user.Id);
        }
        /// <summary>
        /// Change the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>The value of the user id</returns>
        public async Task<Result<string>> ChangePasswordAsync(string userId, 
            string currentPassword, string newPassword, ClientInfo clientInfo)
        {

           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(currentPassword, "currentPassword");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(newPassword, "newPassword");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();


            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByIdAsync(userId),_logger);
            if (user == null)
               return Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ChangePasswordAsync(user, currentPassword, newPassword),_logger);
            if (!result.Succeeded)
                return Result.Fail(new Error("Change Password failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
           
            var revokeResult = await _refreshTokenService.RevokeAllRefreshTokensAsync(userId,clientInfo?.IpAddress);
            if (revokeResult.IsFailed)
                return revokeResult.ToResult<string>();
            return Result.Ok(user.Id);

        }
        /// <summary>
        /// Reset the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns>The value of the user id</returns>
        public async Task<Result<string>> ResetPasswordAsync(string userId, string token, string newPassword)
        {
          
               var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(token, "token");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(newPassword, "newPassword");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => 
                _userManager.FindByIdAsync(userId),_logger);
                if (user == null) return
                    Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ResetPasswordAsync(user, token, newPassword),_logger);
                if (!result.Succeeded)
                return Result.Fail(new Error("Reset Password failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
            return Result.Ok(user.Id);

        }
        /// <summary>
        /// Generate a password reset token for a user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The value of reset token</returns>
        public async Task<Result<string>> ForgotPasswordAsync(string email)
        {
           
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email), _logger);
                if (user == null)
                return Result.Fail(new Error($"Cannot find user by email({email}) by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var token = await IdentityExceptionHelper.Execute(() => 
            _userManager.GeneratePasswordResetTokenAsync(user), _logger);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError($"Token generation failed by UserManager<ApplicationUser> for email: {email}");
                return Result.Fail(new Error("Token generation failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }
            return Result.Ok(token);


        }

    }
}

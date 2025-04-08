
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.Infrastructure;
using BankDirectoryApi.Infrastructure.Identity;
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
        public PasswordService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            ,ILogger<PasswordService> logger)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userId)
        {
            
                if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("userId is required").WithMetadata("StatusCode",HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(()=>
                _userManager.FindByIdAsync(userId),_logger);
                if (user == null)
                return Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.GeneratePasswordResetTokenAsync(user),_logger);
                if (string.IsNullOrEmpty(result)) 
                return Result.Fail(new Error("Token generation failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            return Result.Ok(result);
         
        }
        public async Task<Result<string>> CheckPasswordSignInAsync(LoginUserDTO model, string password, bool lockoutOnFailure)
        {
            
                ApplicationUser? user;
                if (model == null)
                return Result.Fail(new Error("model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(password))
                return Result.Fail(new Error("Password is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (!string.IsNullOrEmpty(model.Email))
            {
                user = await IdentityExceptionHelper.Execute(() =>
                _userManager.FindByEmailAsync(model.Email), _logger);
                if (user == null)
                    return Result.Fail(new Error($"Cannot find user by email({model.Email}) by UserManager<ApplicationUser>")
                        .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            else if (!string.IsNullOrEmpty(model.UserName))
                {
                    user = await IdentityExceptionHelper.Execute(() => 
                    _userManager.FindByNameAsync(model.UserName), _logger);
                    if (user == null)
                    return Result.Fail(new Error($"Cannot find user by Username({model.UserName}) by UserManager<ApplicationUser>")
                      .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
                else if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    user = await IdentityExceptionHelper.Execute(() => 
                    _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber),_logger);
                    if (user == null) 
                    return  Result.Fail(new Error($"Cannot find user by PhoneNumber({model.PhoneNumber}) by UserManager<ApplicationUser>")
                        .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
                else
                {
                   return Result.Fail(new Error("Either Email, UserName or PhoneNumber is required")
                        .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }

                var result = await IdentityExceptionHelper.Execute(() => 
                _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure),_logger);
            if (!result.Succeeded)
            {
               return Result.Fail(new Error("Check Password SignIn failed by SignInManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.Unauthorized)).IncludeIdentityErrors(result);
            }
            return Result.Ok(user.Id);
        }
        public async Task<Result<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {

            if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("User Id is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(currentPassword)) 
                return Result.Fail(new Error("Current password is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(newPassword))
                return Result.Fail(new Error("New password is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByIdAsync(userId),_logger);
            if (user == null)
               return Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ChangePasswordAsync(user, currentPassword, newPassword),_logger);
            if (!result.Succeeded)
                return Result.Fail(new Error("Change Password failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            return Result.Ok(user.Id);

        }
        public async Task<Result<string>> ResetPasswordAsync(string userId, string token, string newPassword)
        {
          
                if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("User Id is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(token))
                return Result.Fail(new Error("Token is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(newPassword)) 
                return Result.Fail(new Error("New password is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));

                var user = await IdentityExceptionHelper.Execute(() => 
                _userManager.FindByIdAsync(userId),_logger);
                if (user == null) return
                    Result.Fail(new Error($"Cannot find user by id({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ResetPasswordAsync(user, token, newPassword),_logger);
                if (!result.Succeeded)
                return Result.Fail(new Error("Reset Password failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            return Result.Ok(user.Id);

        }
        public async Task<Result<string>> ForgotPasswordAsync(string email)
        {
           
                if (string.IsNullOrEmpty(email))
                return Result.Fail(new Error("Email is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email), _logger);
                if (user == null)
                return Result.Fail(new Error($"Cannot find user by email({email}) by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var token = await IdentityExceptionHelper.Execute(() => 
            _userManager.GeneratePasswordResetTokenAsync(user), _logger);
                if (string.IsNullOrEmpty(token))
                return Result.Fail(new Error("Token generation failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            return Result.Ok(token);


        }

    }
}


using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class PasswordService : IPasswordService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public PasswordService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {

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
        public async Task<bool> CheckPasswordSignInAsync(LoginUserDTO user, string password, bool lockoutOnFailure)
        {
            try
            {
                IdentityUser? identityUser;
                if (user == null) throw new Exception("User is required");
                if (string.IsNullOrEmpty(password)) throw new Exception("Password is required");
                if (!string.IsNullOrEmpty(user.Email))
                {
                    identityUser = await _userManager.FindByEmailAsync(user.Email);
                    if (identityUser == null) throw new Exception($"Cannot find user by email({user.Email}) by UserManager<IdentityUser>");
                }
                else if (!string.IsNullOrEmpty(user.UserName))
                {
                    identityUser = await _userManager.FindByNameAsync(user.UserName);
                    if (identityUser == null) throw new Exception($"Cannot find user by user name({user.UserName}) by UserManager<IdentityUser>");
                }
                else if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    identityUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
                    if (identityUser == null) throw new Exception($"Cannot find user by phone number({user.PhoneNumber}) by UserManager<IdentityUser>");
                }
                else
                {
                    throw new Exception("User Name or Email or Phone number is required");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure);
                return result.Succeeded;

            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Check Password Sign In failed", ex);
            }
        }
        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new Exception("User Id is required");
                if (string.IsNullOrEmpty(currentPassword)) throw new Exception("Current password is required");
                if (string.IsNullOrEmpty(newPassword)) throw new Exception("New password is required");
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
                var result = await _userManager.ChangePasswordAsync(identityUser, currentPassword, newPassword);
                if (!result.Succeeded) throw new Exception("Change Password failed by UserManager<IdentityUser>");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new PasswordServiceException("Change Password failed", ex);
            }
        }
        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new Exception("User Id is required");
                if (string.IsNullOrEmpty(token)) throw new Exception("Token is required");
                if (string.IsNullOrEmpty(newPassword)) throw new Exception("New password is required");
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
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

    }
}

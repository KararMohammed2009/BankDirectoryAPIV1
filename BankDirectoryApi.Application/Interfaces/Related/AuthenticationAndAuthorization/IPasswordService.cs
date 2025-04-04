using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
     public interface IPasswordService
    {
        
         Task<Result<string>> CheckPasswordSignInAsync(LoginUserDTO model, string password, bool lockoutOnFailure);
         Task<Result<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
         Task<Result<string>> ResetPasswordAsync(string userId, string token, string newPassword);
         Task<Result<string>> GeneratePasswordResetTokenAsync(string userId);
         Task<Result<string>> ForgotPasswordAsync(string email);
    }
}

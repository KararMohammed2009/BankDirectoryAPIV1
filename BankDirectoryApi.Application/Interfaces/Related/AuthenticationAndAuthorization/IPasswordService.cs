using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IPasswordService
    {
        
        public Task<bool> CheckPasswordSignInAsync(LoginUserDTO model, string password, bool lockoutOnFailure);
        public Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        public Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
        public Task<string> GeneratePasswordResetTokenAsync(string userId);
        public Task<string> ForgotPasswordAsync(string email);
    }
}

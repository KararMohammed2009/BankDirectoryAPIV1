using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IPasswordService
    {
        
        public Task<bool> CheckPasswordSignInAsync(UserDTO user, string password, bool lockoutOnFailure);
        public Task<bool> ChangePasswordAsync(UserDTO user, string currentPassword, string newPassword);
        public Task<bool> ResetPasswordAsync(UserDTO user, string token, string newPassword);
        public Task<string> GeneratePasswordResetTokenAsync(string userId);
        public Task<string> ForgotPasswordAsync(string email);
    }
}

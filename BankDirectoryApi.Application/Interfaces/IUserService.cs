using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourProject.Infrastructure.Identity;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IUserService
    {
        public Task<AuthDTO> ExternalLoginAsync(string code);


        public Task<AuthDTO?> RegisterAsync(RegisterUserDTO model);
        public Task<AuthDTO?> LoginAsync(LoginUserDTO model);
        public Task<IEnumerable<UserDTO>?> GetAllUsersAsync();
        public Task<UserDTO?> GetUserByIdAsync(string userId);
        public Task<bool?> UpdateUserAsync(UpdateUserDTO model);
        public Task<bool?> AssignRoleAsync(string userId, string role);
        public Task<bool?> ForgotPasswordAsync(ForgotPasswordDTO model);
        public Task<bool?> ResetPasswordAsync(ResetPasswordDTO model);
        public Task<bool?> ChangePasswordAsync(ChangePasswordDTO model);
        public Task<bool?> ConfirmEmailAsync(EmailConfirmationDTO model);
        public Task<bool?> ResendConfirmationEmailAsync(string email);
        public Task<bool?> Logout(string email); //Token Blacklisting
        public Task<bool?> DeleteUserAsync(string userId);
        public Task<IEnumerable<string>?> DeleteUsersAsync(IEnumerable<string> userIds);
        public Task<string?> RefreshTokenAsync(string refreshToken);

    }
}

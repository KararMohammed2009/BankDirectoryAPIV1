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

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IUserService
    {
        public Task<AuthDTO?> ExternalLoginAsync(string code, ClientInfo clientInfo);


        public Task<(string? accessToken, string? refreshToken)> GenerateAndStoreTokensAsync(
           IdentityUser user
          , ClientInfo clientInfo);
        public Task<AuthDTO?> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo);
        public Task<AuthDTO?> LoginAsync(LoginUserDTO model, ClientInfo clientInfo);
        public Task<IEnumerable<UserDTO>?> GetAllUsersAsync();
        public Task<UserDTO?> GetUserByIdAsync(string userId);
        public Task<bool?> UpdateUserAsync(UpdateUserDTO model);
        public Task<bool?> AssignRoleAsync(string userId, string role);
        public Task<bool?> ForgotPasswordAsync(ForgotPasswordDTO model);
        public Task<AuthDTO?> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo);
        public Task<AuthDTO?> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo);
        public Task<bool?> ConfirmEmailAsync(EmailConfirmationDTO model);
        public Task<bool?> ResendConfirmationEmailAsync(string email);
        public Task<bool?> Logout(string userId, string refreshToken, ClientInfo clientInfo); //Token Blacklisting
        public Task<bool?> DeleteUserAsync(string userId);
        public Task<IEnumerable<string>?> DeleteUsersAsync(IEnumerable<string> userIds);
        public Task<AuthDTO?> GenerateAccessTokenFromRefreshTokenAsync(string userId,string refreshToken, ClientInfo clientInfo);

    }
}

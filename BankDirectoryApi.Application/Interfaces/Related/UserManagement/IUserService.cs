
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.UserManagement
{
    public interface IUserService
    {

        public Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        public Task<UserDTO> GetUserByIdAsync(string userId);
        public Task<UserDTO> GetUserByEmailAsync(string email);
        public Task<UserDTO> GetUserByUserNameAsync(string userName);
        public Task<UserDTO?> UserExistsByEmailAsync(string email);
        public Task<UserDTO> UpdateUserAsync(UpdateUserDTO model);
        public Task<bool> DeleteUserAsync(string userId);
        public Task<UserDTO> CreateUserAsync(RegisterUserDTO model);
        public Task<bool> ConfirmEmailAsync(string email, string token);
        public Task<bool> IsEmailConfirmedAsync(UserDTO model);
        public Task<string> GenerateEmailConfirmationTokenAsync(string email);
        public Task<bool> AddLoginAsync(string id,string email, string name,string externalAccessToken,string providerName);
        public  Task<bool> SetTwoFactorAuthenticationAsync(string userId, bool enabled);
        public Task<Dictionary<string, string>> GetUserCalimsAsync(string userId);

    }
}


using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentResults;
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

         Task<Result<List<UserDTO>>> GetAllUsersAsync(UserFilterDTO model,CancellationToken cancellationToken);
         Task<Result<UserDTO>> GetUserByIdAsync(string userId);
         Task<Result<UserDTO>> GetUserByEmailAsync(string email);
         Task<Result<UserDTO>> GetUserByUserNameAsync(string userName);
         Task<Result<UserDTO>> UserExistsByEmailAsync(string email);
         Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model);
         Task<Result<string>> DeleteUserAsync(string userId);
         Task<Result<UserDTO>> CreateUserAsync(RegisterUserDTO model);
         Task<Result<string>> ConfirmEmailAsync(string email, string token);
         Task<Result<bool>> IsEmailConfirmedAsync(UserDTO model);
         Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email);
         Task<Result<string>> AddLoginAsync(string id,string email, string name,string externalAccessToken,string providerName);
          Task<Result<string>> SetTwoFactorAuthenticationAsync(string userId, bool enabled);
         Task<Result<Dictionary<string, string>>> GetUserCalimsAsync(string userId);

    }
}

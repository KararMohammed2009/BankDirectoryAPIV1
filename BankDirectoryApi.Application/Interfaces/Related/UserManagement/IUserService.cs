
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.UserManagement
{
    public interface IUserService
    {

        public Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        public Task<UserDTO> GetUserByIdAsync(string userId);
        public Task<UserDTO> GetUserByEmailAsync(string email);
        public Task<UserDTO> UpdateUserAsync(UpdateUserDTO model);
        public Task<bool> DeleteUserAsync(string userId);
        public Task<UserDTO> CreateUserAsync(RegisterUserDTO model);
        public Task<bool> CheckPasswordSignInAsync(UserDTO user , string password,bool lockoutOnFailure);

    }
}

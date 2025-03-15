using BankDirectoryApi.Application.DTOs.Generic;
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

        public Task<Result<IEnumerable<UserDTO>>> GetAllUsersAsync();
        public Task<Result<UserDTO>> GetUserByIdAsync(string userId);
        public Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model);
        public Task<Result<string>> DeleteUserAsync(string userId);
        public Task<Result<string>> CreateUserAsync(CreateUserDTO model);

    }
}

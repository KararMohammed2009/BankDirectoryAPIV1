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
        
        public Task<Result<IEnumerable<UserDTO>>> GetAllUsersAsync();
        public Task<Result<UserDTO>> GetUserByIdAsync(string userId);
        public Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model);
        public Task<Result<string>> DeleteUserAsync(string userId);
        public Task<Result<string>> CreateUserAsync(CreateUserDTO model);

    }
}

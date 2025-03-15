using BankDirectoryApi.Application.DTOs.Generic;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IRoleService
    {
        public Task<Result<bool>> AssignRoleAsync(string userId, string role);
        public Task<Result<bool>> RemoveRoleAsync(string userId, string role);
        public Task<Result<IEnumerable<string>>> GetRolesAsync(string userId);
        public Task<Result<IEnumerable<string>>> GetAllRolesAsync();
        public Task<Result<bool>> RoleExistsAsync(string role);
        public Task<Result<string>> CreateRoleAsync(string role);
        public Task<Result<string>> DeleteRoleAsync(string role);

    }
}

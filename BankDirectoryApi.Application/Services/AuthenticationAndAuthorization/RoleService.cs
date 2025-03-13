using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.Interfaces.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.AuthenticationAndAuthorization
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleService(UserManager<IdentityUser> userManager ,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<Result<bool>> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult(new List<Error> { new Error { Message = "User not found", Severity = Severity.Error, Code = "UserNotFound" } });

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Role assignment failed", Severity = Severity.Error, Code = "RoleAssignmentFailed" } });
        }
        public async Task<Result<bool>> RemoveRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult(new List<Error> { new Error { Message = "User not found", Severity = Severity.Error, Code = "UserNotFound" } });
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Role removal failed", Severity = Severity.Error, Code = "RoleRemovalFailed" } });
        }
        public async Task<Result<IEnumerable<string>>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<IEnumerable<string>>.FailureResult(new List<Error> { new Error { Message = "User not found", Severity = Severity.Error, Code = "UserNotFound" } });
            var roles = await _userManager.GetRolesAsync(user);
            return Result<IEnumerable<string>>.SuccessResult(roles);
        }
        public async Task<Result<IEnumerable<string>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(o => o.Name).ToListAsync();
            if (roles == null || !roles.Any())
                return Result<IEnumerable<string>>.FailureResult(new List<Error> { new Error { Message = "No roles found", Severity = Severity.Error, Code = "NoRolesFound" } });
            return Result<IEnumerable<string>>.SuccessResult(roles!);
        }
        public async Task<Result<bool>> RoleExistsAsync(string role)
        {
            var exists = await _roleManager.RoleExistsAsync(role);
            return Result<bool>.SuccessResult(exists);
        }
        public async Task<Result<string>> CreateRoleAsync(string role)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            return result.Succeeded ? Result<string>.SuccessResult(role) : Result<string>.FailureResult(
                new List<Error> { new Error { Message = "Role creation failed", Severity = Severity.Error, Code = "RoleCreationFailed" } });
        }
        public async Task<Result<string>> DeleteRoleAsync(string role)
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity == null)
                return Result<string>.FailureResult(new List<Error> { new Error { Message = "Role not found", Severity = Severity.Error, Code = "RoleNotFound" } });
            var result = await _roleManager.DeleteAsync(roleEntity);
            return result.Succeeded ? Result<string>.SuccessResult(role) : Result<string>.FailureResult(
                new List<Error> { new Error { Message = "Role deletion failed", Severity = Severity.Error, Code = "RoleDeletionFailed" } });
        }
    }
}

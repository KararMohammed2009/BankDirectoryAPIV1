
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
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
        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User Id is required");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception($"User({userId}) not found by UserManager<IdentityUser>");
                }
                var result = await _userManager.AddToRoleAsync(user, role);
                if(!result.Succeeded)
                {
                    throw new Exception($"Role assignment for user({userId}) failed by UserManager<IdentityUser>");
                }
                return true;

            } catch (Exception ex)
            {
                throw new RoleServiceException("Assign Role failed", ex);
            } 
        }
        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User Id is required");
                if (string.IsNullOrEmpty(role))
                    throw new Exception("Role name is required");
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception($"User({userId}) not found by UserManager<IdentityUser>");
                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Role removal for user({userId}) failed by UserManager<IdentityUser>");
                }
                return true;
            }
            catch (Exception ex) { 
                throw new RoleServiceException("Remove Role failed", ex);
            }
        }
        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User Id is required");
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception($"User({userId}) not found by UserManager<IdentityUser>");
                var roles = await _userManager.GetRolesAsync(user);
                if(roles ==null)
                    throw new Exception($"Roles for user({userId}) not found by UserManager<IdentityUser>");
                return roles;
            }
            catch (Exception ex)
            {
                throw new RoleServiceException("Get Roles failed", ex);
            }
           
        }
        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.Select(o => o.Name!).ToListAsync();
                if (roles == null)
                    throw new Exception("Roles not found by RoleManager<IdentityRole>");
                return roles;
            }
            catch (Exception ex)
            {
                throw new RoleServiceException("Get all roles failed", ex);
            }
        }
        public async Task<bool> RoleExistsAsync(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                    throw new Exception("Role name is required");
                var exists = await _roleManager.RoleExistsAsync(role);
                return exists;
            }
            catch(Exception ex)
            {
                throw new RoleServiceException("Role exists check failed", ex);
            }
        }
        public async Task<string> CreateRoleAsync(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                    throw new Exception("Role name is required");
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if(!result.Succeeded)
                    throw new Exception("Role creation failed by RoleManager<IdentityRole>");
                return role;
            }
            catch(Exception ex)
            {
                throw new RoleServiceException("Role creation failed", ex);
            }
        }
        public async Task<bool> DeleteRoleAsync(string role)
        {
            try
            {
                if(role == null)
                    throw new Exception("Role name is required");
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity == null)
                    throw new Exception($"Role({role}) not found by RoleManager<IdentityRole>");
                var result = await _roleManager.DeleteAsync(roleEntity);
                if (!result.Succeeded)
                    throw new Exception($"Role deletion failed by RoleManager<IdentityRole>");
                return true;
            }
            catch(Exception ex)
            {
                throw new RoleServiceException("Role deletion failed", ex);
            }
           
        }
    }
}

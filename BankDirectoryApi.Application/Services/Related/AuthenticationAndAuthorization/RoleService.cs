using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Infrastructure;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Domain.Entities.Identity;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user roles
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RoleService> _logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        public RoleService(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager, ILogger<RoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns>The value of the user id</returns>
        public async Task<Result<string>> AssignRoleAsync(string userId, string roleName)
        {
            var validationResult =  ValidationHelper.ValidateNullOrWhiteSpaceString(userId,"userId");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(roleName, "role");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
            {
                return Result.Fail(new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
            var roleExists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(roleName), _logger);
            if (!roleExists)
            {
                return Result.Fail(new Error($"Role({roleName}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
            var result = await IdentityExceptionHelper.Execute(() => _userManager.AddToRoleAsync(user, roleName), _logger);
            if (!result.Succeeded)
            {
                _logger.LogError($"Role assignment failed by UserManager<ApplicationUser> : userId ={userId} , roleName ={roleName}"
                    ,result.Errors);
                return Result.Fail(new Error($"Role assignment for user({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
            }
            return Result.Ok(userId);
        }
        /// <summary>
        /// Remove a role from a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns>The value of the user id</returns>
        public async Task<Result<string>> RemoveRoleAsync(string userId, string roleName)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(roleName, "role");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
                return Result.Fail(new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var roleExists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(roleName), _logger);
            if (!roleExists)
                return Result.Fail(new Error($"Role({roleName}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var result = await IdentityExceptionHelper.Execute(() => _userManager.RemoveFromRoleAsync(user, roleName), _logger);
            if (!result.Succeeded)
            {
                _logger.LogError($"Role removal failed by UserManager<ApplicationUser> : userId ={userId} , roleName ={roleName}",
                    result.Errors);
                return Result.Fail(new Error($"Role removal for user({userId}) failed by UserManager<ApplicationUser>")
                      .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
            }
            return Result.Ok(userId);

        }
        /// <summary>
        /// Get all roles of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of the user roles</returns>
        public async Task<Result<List<string>>> GetRolesAsync(string userId)
        {

           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<List<string>>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
                return Result.Fail
                     (new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                     .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            if (roles == null)
                return Result.Fail
                    (new Error($"Roles not found for user({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            return Result.Ok(roles.ToList());
        }
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The value of all roles</returns>
        public async Task<Result<List<string>>> GetAllRolesAsync( CancellationToken cancellationToken)
        {

            var roles = await IdentityExceptionHelper.Execute(() => 
            _roleManager.Roles.Select(o => o.Name!).ToListAsync(cancellationToken), _logger);
            if (roles == null)
                return Result.Fail
                       (new Error("Roles not found by RoleManager<ApplicationRole>")
                       .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            return Result.Ok(roles);

        }
        /// <summary>
        /// Check if a role exists
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>True if the role exists, false otherwise</returns>
        public async Task<Result<bool>> RoleExistsAsync(string roleName)
        {

           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(roleName, "role");
            if (validationResult.IsFailed) return validationResult.ToResult<bool>();

            var exists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(roleName), _logger);
            return Result.Ok(exists);

        }
        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>The value of the role id</returns>
        public async Task<Result<string>> CreateRoleAsync(string roleName)
        {


            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(roleName, "role");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();

            var roleExists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(roleName), _logger);
            if (roleExists)
                return Result.Fail
                    (new Error($"Role({roleName}) already exists by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceAlreadyExists));
            var roleEntity = new ApplicationRole(roleName);
            var result = await IdentityExceptionHelper.Execute(() =>
            _roleManager.CreateAsync(roleEntity), _logger);
            if (!result.Succeeded)
            {
                _logger.LogError($"Role creation failed by RoleManager<ApplicationRole> : roleName ={roleName}"
                    , result.Errors);
                return Result.Fail
                    (new Error($"Role creation failed by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
            }
            return Result.Ok(roleEntity.Id);

        }
        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>The value of the role id</returns>
        public async Task<Result<string>> DeleteRoleAsync(string roleName)
        {

         var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(roleName, "role");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();

            var roleEntity = await IdentityExceptionHelper.Execute(() => _roleManager.FindByNameAsync(roleName), _logger);
            if (roleEntity == null)
                return Result.Fail
                    (new Error($"Role({roleName}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result = await IdentityExceptionHelper.Execute(() => _roleManager.DeleteAsync(roleEntity), _logger);
            if (!result.Succeeded)
            {
                _logger.LogError($"Role deletion failed by RoleManager<ApplicationRole> : roleName ={roleName}"
                    , result.Errors);
                return Result.Fail
                    (new Error($"Role deletion failed by RoleManager<ApplicationRole>")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError)).IncludeIdentityErrors(result);
            }
            return Result.Ok(roleEntity.Id);



        }
    }
}

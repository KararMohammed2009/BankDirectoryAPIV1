using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Infrastructure;
using BankDirectoryApi.Infrastructure.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using BankDirectoryApi.Common.Extensions;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RoleService> _logger;
        public RoleService(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager, ILogger<RoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Result<string>> AssignRoleAsync(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Fail(new Error("User Id is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            if (string.IsNullOrEmpty(role))
            {
                return Result.Fail(new Error("Role name is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
            {
                return Result.Fail(new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            }
            var roleExists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(role), _logger);
            if (!roleExists)
            {
                return Result.Fail(new Error($"Role({role}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            }
            var result = await IdentityExceptionHelper.Execute(() => _userManager.AddToRoleAsync(user, role), _logger);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error($"Role assignment for user({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            }
            return Result.Ok(userId);
        }
        public async Task<Result<string>> RemoveRoleAsync(string userId, string role)
        {

            if (string.IsNullOrEmpty(userId))
                return Result.Fail(new Error("User Id is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(role))
                return Result.Fail(new Error("Role name is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
                return Result.Fail(new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            var roleExists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(role), _logger);
            if (!roleExists)
                return Result.Fail(new Error($"Role({role}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            var result = await IdentityExceptionHelper.Execute(() => _userManager.RemoveFromRoleAsync(user, role), _logger);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error($"Role removal for user({userId}) failed by UserManager<ApplicationUser>")
                      .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            }
            return Result.Ok(userId);

        }
        public async Task<Result<List<string>>> GetRolesAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return Result.Fail
                    (new Error("User Id is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId), _logger);
            if (user == null)
                return Result.Fail
                     (new Error($"User({userId}) not found by UserManager<ApplicationUser>")
                     .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            if (roles == null)
                return Result.Fail
                    (new Error($"Roles not found for user({userId}) by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            return Result.Ok(roles.ToList());
        }
        public async Task<Result<List<string>>> GetAllRolesAsync( CancellationToken cancellationToken)
        {

            var roles = await IdentityExceptionHelper.Execute(() => 
            _roleManager.Roles.Select(o => o.Name!).ToListAsync(cancellationToken), _logger);
            if (roles == null)
                return Result.Fail
                       (new Error("Roles not found by RoleManager<ApplicationRole>")
                       .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            return Result.Ok(roles);

        }
        public async Task<Result<bool>> RoleExistsAsync(string role)
        {

            if (string.IsNullOrEmpty(role))
                return Result.Fail
                        (new Error("Role name is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var exists = await IdentityExceptionHelper.Execute(() => _roleManager.RoleExistsAsync(role), _logger);
            return Result.Ok(exists);

        }
        public async Task<Result<string>> CreateRoleAsync(string role)
        {

            if (string.IsNullOrEmpty(role))
                return Result.Fail
                    (new Error("Role name is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var result = await IdentityExceptionHelper.Execute(() => _roleManager.CreateAsync(new ApplicationRole(role)), _logger);
            if (!result.Succeeded)
                return Result.Fail
                    (new Error($"Role creation failed by RoleManager<ApplicationRole>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            return Result.Ok(role);

        }
        public async Task<Result<string>> DeleteRoleAsync(string role)
        {

            if (string.IsNullOrEmpty(role))
                return Result.Fail
                     (new Error("Role name is required").WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var roleEntity = await IdentityExceptionHelper.Execute(() => _roleManager.FindByNameAsync(role), _logger);
            if (roleEntity == null)
                return Result.Fail
                    (new Error($"Role({role}) not found by RoleManager<ApplicationRole>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));

            var result = await IdentityExceptionHelper.Execute(() => _roleManager.DeleteAsync(roleEntity), _logger);
            if (!result.Succeeded)
                return Result.Fail
                    (new Error($"Role deletion failed by RoleManager<ApplicationRole>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            return Result.Ok(role);



        }
    }
}

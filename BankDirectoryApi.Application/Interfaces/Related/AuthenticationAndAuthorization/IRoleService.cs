
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user roles
    /// </summary>
    interface IRoleService
    {
        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> AssignRoleAsync(string userId, string role);
        /// <summary>
        /// Remove a role from a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns>The value of the user id</returns>
        Task<Result<string>> RemoveRoleAsync(string userId, string role);
        /// <summary>
        /// Get all roles of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of the user roles</returns>
        Task<Result<List<string>>> GetRolesAsync(string userId);
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The value of all roles</returns>
        Task<Result<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Check if a role exists
        /// </summary>
        /// <param name="role"></param>
        /// <returns>True if the role exists, false otherwise</returns>
        Task<Result<bool>> RoleExistsAsync(string roleName);
        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns> The value of the role id</returns>
        Task<Result<string>> CreateRoleAsync(string roleName);
        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>The value of the role id</returns>
        Task<Result<string>> DeleteRoleAsync(string roleName);

    }
}

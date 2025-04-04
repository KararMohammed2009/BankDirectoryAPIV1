
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
     interface IRoleService
    {
         Task<Result<string>> AssignRoleAsync(string userId, string role);
         Task<Result<string>> RemoveRoleAsync(string userId, string role);
         Task<Result<List<string>>> GetRolesAsync(string userId);
         Task<Result<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken);
         Task<Result<bool>> RoleExistsAsync(string role);
         Task<Result<string>> CreateRoleAsync(string role);
         Task<Result<string>> DeleteRoleAsync(string role);

    }
}

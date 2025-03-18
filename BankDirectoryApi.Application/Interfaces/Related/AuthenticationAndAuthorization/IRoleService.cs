namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IRoleService
    {
        public Task<bool> AssignRoleAsync(string userId, string role);
        public Task<bool> RemoveRoleAsync(string userId, string role);
        public Task<IEnumerable<string>> GetRolesAsync(string userId);
        public Task<IEnumerable<string>> GetAllRolesAsync();
        public Task<bool> RoleExistsAsync(string role);
        public Task<string> CreateRoleAsync(string role);
        public Task<bool> DeleteRoleAsync(string role);

    }
}

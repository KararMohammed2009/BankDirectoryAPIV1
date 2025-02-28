using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities; // Assuming your User entity is here

namespace BankDirectoryApi.Infrastructure.Identity
{
    public interface IIdentityService
    {
        Task<string> GenerateJwtToken(User user);
        string GenerateJwtRefreshToken(User user);
        // Add other identity-related methods here (e.g., ValidateToken)
    }
}
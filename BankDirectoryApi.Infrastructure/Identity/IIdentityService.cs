using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity; // Assuming your User entity is here

namespace BankDirectoryApi.Infrastructure.Identity
{
    public interface IIdentityService
    {
        Task<string> GenerateJwtToken(IdentityUser user);
        string GenerateJwtRefreshToken(IdentityUser user);
        // Add other identity-related methods here (e.g., ValidateToken)
    }
}
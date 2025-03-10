using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity; // Assuming your User entity is here

namespace BankDirectoryApi.Infrastructure.Identity
{
    public interface IJwtService
    {
        Task<string?> GenerateJwtTokenAsync(IdentityUser user);
        Task<string?> GenerateJwtRefreshTokenAsync(IdentityUser user);
        Task<string?> GenerateJwtTokenFromRefreshTokenAsync(string refreshToken);
        Task<string?> InvalidateRefreshTokenAsync(string refreshToken); // eg. security breach is detected
        Task<string?> RevokeRefreshTokenAsync(string refreshToken); // eg. user logs out
        Task<string?> UseRefreshTokenAsync(string refreshToken); // eg. exchanging a refresh token

        // Add other identity-related methods here (e.g., ValidateToken)
    }
}
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity; // Assuming your User entity is here

namespace BankDirectoryApi.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        Task<string?> GenerateAccessTokenAsync(IdentityUser user);
        Task<(string? RefreshToken, string? HashedRefreshToken)> GenerateRefreshTokenAsync(IdentityUser user);
        Task<bool?> ValidateAccessTokenAsync(string accessToken);
        string GenerateNewSessionIdAsync();
    }
}
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity; // Assuming your User entity is here

namespace BankDirectoryApi.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        Task<string?> GenerateAccessTokenAsync(IdentityUser user);
        Task<string?> GenerateRefreshTokenAsync(IdentityUser user);
        Task<bool?> ValidateTokenAsync(string token);
    }
}
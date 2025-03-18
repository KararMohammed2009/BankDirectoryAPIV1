using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IRefreshTokenService
    {
         (string RefreshToken, string HashedRefreshToken) GenerateRefreshTokenAsync();
        Task<(RefreshToken refreshTokenEntity, string refreshToken)> GenerateRefreshTokenEntityAsync( string userId , ClientInfo clientInfo);
        Task<RefreshToken> StoreRefreshTokenAsync( RefreshToken refreshToken );
        Task<bool> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken);
        Task<bool> RevokeAllRefreshTokensAsync(string userId,string sessionId,string? ipAddress);
    }
}

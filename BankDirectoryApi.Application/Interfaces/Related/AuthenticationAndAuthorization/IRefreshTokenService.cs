using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Classes;
using BankDirectoryApi.Domain.Entities;
using FluentResults;
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
        Result<(string RefreshToken, string HashedRefreshToken)> GenerateRefreshTokenAsync();
        Task<Result<(RefreshToken refreshTokenEntity, string refreshToken)>> GenerateRefreshTokenEntityAsync( string userId , ClientInfo clientInfo);
        Task<Result<RefreshToken>> StoreRefreshTokenAsync( RefreshToken refreshToken );
        Task<Result<RefreshToken>> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken);
        Task<Result<string>> RevokeAllRefreshTokensAsync(string userId,string sessionId,string? ipAddress);
    }
}

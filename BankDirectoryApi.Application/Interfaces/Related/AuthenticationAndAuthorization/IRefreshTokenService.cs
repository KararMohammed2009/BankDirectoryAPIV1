using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Domain.Entities;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to handle refresh tokens
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <returns>The refresh token and its hash</returns>
        Result<(string RefreshToken, string HashedRefreshToken)> GenerateRefreshToken();
        /// <summary>
        /// Generate a new refresh token entity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The refresh token entity and the refresh token</returns>
        Result<(RefreshToken refreshTokenEntity, string refreshToken)> GenerateRefreshTokenEntity( string userId , ClientInfo clientInfo);
        /// <summary>
        /// Store the refresh token in the database
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>The stored refresh token entity</returns>
        Task<Result<RefreshToken>> StoreRefreshTokenAsync( RefreshToken refreshToken );
        /// <summary>
        /// Rotate the refresh token
        /// </summary>
        /// <param name="oldRefreshToken"></param>
        /// <param name="newRefreshToken"></param>
        /// <returns>The rotated new refresh token entity</returns>
        Task<Result<RefreshToken>> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken);
        /// <summary>
        /// Revoke a refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="ipAddress"></param>
        /// <returns>The value of user id</returns>
        Task<Result<string>> RevokeAllRefreshTokensAsync(string userId,string sessionId,string? ipAddress);
    }
}

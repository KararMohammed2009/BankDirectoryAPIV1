using BankDirectoryApi.Domain.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for managing refresh tokens
    /// </summary>
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="RevokedByIp"></param>
        /// <returns>The status of the operation</returns>
        public Task<Result> RevokeAllRefreshTokensAsync(string userId, string sessionId ,string? RevokedByIp ); // eg. user logs out
        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="RevokedByIp"></param>
        /// <returns>The status of the operation</returns>
        public Task<Result> RevokeAllRefreshTokensAsync(string userId, string RevokedByIp); // eg. user change password
        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The status of the operation</returns>
        public Task<Result> RevokeAllRefreshTokensAsync(string userId);
        /// <summary>
        /// Rotate the refresh token with the given hash by the new resfresh token
        /// </summary>
        /// <param name="oldTokenHash"></param>
        /// <param name="newToken"></param>
        /// <returns>The status of the operation</returns>
        public Task<Result> RotateRefreshTokenAsync(string oldTokenHash, RefreshToken newToken); // eg. user request for new accessToken


    }
}

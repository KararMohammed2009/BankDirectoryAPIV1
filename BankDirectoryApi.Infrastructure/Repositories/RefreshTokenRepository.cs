using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Common.Services;
using System.Linq.Expressions;
using FluentResults;
using System.Net;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Errors;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing refresh tokens
    /// </summary>
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dateTimeProvider"></param>
        public RefreshTokenRepository(ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context) // Pass the context to the base class constructor
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Check if the refresh token is valid
        /// </summary>
        /// <returns>Expression to check if the refresh token is valid</returns>
        private Expression<Func<RefreshToken, bool>> IsValidToken()
        {
            return rt => !rt.IsRevoked
                      && !rt.IsUsed
                      && rt.ExpirationDate > _dateTimeProvider.UtcNow.Value;
        }

        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="RevokedByIp"></param>
        /// <returns>The status of the operation</returns>
        public async Task<Result> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? RevokedByIp)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult;
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(sessionId, "sessionId");
            if (validationResult.IsFailed)
                return validationResult;

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.SessionId == sessionId)
                .Where(IsValidToken()).ToListAsync();

            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
            return Result.Ok();
        }
        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="RevokedByIp"></param>
        /// <returns>The status of the operation</returns>
        public async Task<Result> RevokeAllRefreshTokensAsync(string userId, string RevokedByIp)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult;

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
           
            return Result.Ok();
        }
        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The status of the operation</returns>
        public async Task<Result> RevokeAllRefreshTokensAsync(string userId)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult;

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
            });
           
            return Result.Ok();
        }
        /// <summary>
        /// Rotate the refresh token with the given hash by the new refresh token
        /// </summary>
        /// <param name="oldTokenHash"></param>
        /// <param name="newToken"></param>
        /// <returns>The status of the operation</returns>
        public async Task<Result> RotateRefreshTokenAsync(string oldTokenHash, RefreshToken newToken)
        {
           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(oldTokenHash, "oldTokenHash");
            if (validationResult.IsFailed)
                return validationResult;
            validationResult = ValidationHelper.ValidateNullModel(newToken, "newToken");
            if (validationResult.IsFailed)
                return validationResult;

          
            var refreshToken = await _context.RefreshTokens.Where(rt =>
            rt.TokenHash == oldTokenHash).Where(IsValidToken()).FirstOrDefaultAsync();
            if (refreshToken == null)
                return Result.Fail(new Error("Refresh token not found")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            newToken.UserId = refreshToken.UserId;
            _context.RefreshTokens.Add(newToken);

            refreshToken.IsUsed = true;
            refreshToken.ReplacedByTokenHash = newToken.TokenHash;
            return Result.Ok();
        }
    }

}

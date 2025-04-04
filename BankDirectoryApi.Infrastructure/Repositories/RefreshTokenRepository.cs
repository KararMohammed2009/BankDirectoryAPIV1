using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Common.Services;
using System.Linq.Expressions;
using FluentResults;
using System.Net;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>,IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RefreshTokenRepository(ApplicationDbContext context,IDateTimeProvider dateTimeProvider) : base(context)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }
        private  Expression<Func<RefreshToken, bool>> IsValidToken()
        {
            return rt => !rt.IsRevoked
                      && !rt.IsUsed
                      && rt.ExpirationDate > _dateTimeProvider.UtcNow.Value;
        }


        public async Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? RevokedByIp)
        {
            if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("User Id is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (string.IsNullOrEmpty(sessionId))
                return Result.Fail(new Error("Session Id is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
         
                var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.SessionId == sessionId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt => {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected == 0)
                return Result.Fail(new Error("No refresh tokens were revoked")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            if (rowsAffected < 0)
                return Result.Fail(new Error("An error occurred while revoking refresh tokens")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            return Result.Ok(true);
        }
        public async Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId,string RevokedByIp)
        {
            if (string.IsNullOrEmpty(userId))
                return Result.Fail(new Error("User Id is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected == 0)
                return Result.Fail(new Error("No refresh tokens were revoked")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            if (rowsAffected < 0)
                return Result.Fail(new Error("An error occurred while revoking refresh tokens")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            return Result.Ok(true);
        }
        public async Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Result.Fail(new Error("User Id is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
            });
            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected == 0)
                return Result.Fail(new Error("No refresh tokens were revoked")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            if (rowsAffected < 0)
                return Result.Fail(new Error("An error occurred while revoking refresh tokens")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            return Result.Ok(true);
        }
        public async Task<Result<bool>> RotateRefreshTokenAsync(string oldTokenHash,RefreshToken newToken)
        {
            if (string.IsNullOrEmpty(oldTokenHash))
                return Result.Fail(new Error("Old token hash is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (newToken == null)
                return Result.Fail(new Error("New token is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var refreshToken = await _context.RefreshTokens.Where(rt=>
            rt.TokenHash == oldTokenHash).Where(IsValidToken()).FirstOrDefaultAsync();
            if (refreshToken == null)
                return Result.Fail(new Error("Refresh token not found")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            newToken.UserId = refreshToken.UserId;
            _context.RefreshTokens.Add(newToken);

            refreshToken.IsUsed = true;
            refreshToken.ReplacedByTokenHash = newToken.TokenHash;

            var rowsAffected = await _context.SaveChangesAsync();
            if (rowsAffected == 0)
                return Result.Fail(new Error("No refresh tokens were revoked")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            if (rowsAffected < 0)
                return Result.Fail(new Error("An error occurred while revoking refresh tokens")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            return Result.Ok(true);
        }
      

    }

}

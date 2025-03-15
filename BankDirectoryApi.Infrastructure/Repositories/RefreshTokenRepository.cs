using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Common.Services;
using System.Linq.Expressions;

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
                      && rt.ExpirationDate > _dateTimeProvider.UtcNow;
        }


        public async Task<bool> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? RevokedByIp)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.SessionId == sessionId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt => {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RevokeAllRefreshTokensAsync(string userId,string RevokedByIp)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
                rt.RevokedByIp = RevokedByIp;
            });
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RevokeAllRefreshTokensAsync(string userId)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(IsValidToken()).ToListAsync();
            refreshTokens.ForEach(rt =>
            {
                rt.IsRevoked = true;
            });
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RotateRefreshTokenAsync(string oldTokenHash,RefreshToken newToken)
        {
            var refreshToken = await _context.RefreshTokens.Where(rt=>
            rt.TokenHash == oldTokenHash).Where(IsValidToken()).FirstOrDefaultAsync();
            if (refreshToken == null) return false;
            newToken.UserId = refreshToken.UserId;
            _context.RefreshTokens.Add(newToken);

            refreshToken.IsUsed = true;
            refreshToken.ReplacedByTokenHash = newToken.TokenHash;
            await _context.SaveChangesAsync();
            return true;
        }
      

    }

}

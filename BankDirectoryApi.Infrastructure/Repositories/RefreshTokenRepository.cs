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
        public async Task<RefreshToken?> GetByTokenAndInsureValidityAsync(string token)
        {
            return await _context.RefreshTokens.Where(rt =>
            rt.Token == token
            && rt.IsRevoked == false && rt.IsInvalidated == false && rt.IsUsed == false
            && rt.ExpirationDate > _dateTimeProvider.UtcNow
            ).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable< RefreshToken>?> GetByUserIdAndInsureValidityAsync(string userId)
        {
            return await _context.RefreshTokens.Where(rt =>
            rt.UserId == userId
            && rt.IsRevoked == false && rt.IsInvalidated == false && rt.IsUsed == false
            && rt.ExpirationDate > _dateTimeProvider.UtcNow
            ).ToListAsync();
        }
        public async Task<bool?> InvalidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt=>rt.Token == token); // without filtering
            if (refreshToken == null) return null;
            refreshToken.IsInvalidated = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool?> RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await GetByTokenAndInsureValidityAsync(token);
            if (refreshToken == null) return null;
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool?> UseRefreshTokenAsync(string token)
        {
            var refreshToken = await GetByTokenAndInsureValidityAsync(token);
            if (refreshToken == null) return null;
            refreshToken.IsUsed = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool?> RevokeAllRefreshTokensAsync(string userId)
        { 
            var refreshTokens = await GetByUserIdAndInsureValidityAsync(userId);
            if (refreshTokens == null) return null;
            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.IsRevoked = true;
                _context.RefreshTokens.Update(refreshToken);
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }

}

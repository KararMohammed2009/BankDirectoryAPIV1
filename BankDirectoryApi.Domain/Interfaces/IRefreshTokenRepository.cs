using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        public Task<bool?> InvalidateRefreshTokenAsync(string token); // eg. security breach is detected
        public Task<bool?> RevokeRefreshTokenAsync(string token); // eg. user logs out
        public Task<bool?> UseRefreshTokenAsync(string token); // eg. exchanging a refresh token
        public Task<RefreshToken?> GetByTokenAndInsureValidityAsync(string token);
        public Task<bool?> RevokeAllRefreshTokensAsync(string userId); // eg. user logs out
    }
}

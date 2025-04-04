using BankDirectoryApi.Domain.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        public Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId, string sessionId ,string? RevokedByIp ); // eg. user logs out
        public Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId, string RevokedByIp); // eg. user change password
        public Task<Result<bool>> RevokeAllRefreshTokensAsync(string userId);
        public Task<Result<bool>> RotateRefreshTokenAsync(string oldTokenHash, RefreshToken newToken); // eg. user request for new accessToken


    }
}

using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<IEnumerable<RefreshToken>> GetAllRefreshTokensAsync();
        Task<RefreshToken?> GetRefreshTokenByIdAsync(int id);
        Task AddRefreshTokenAsync(RefreshToken RefreshTokens);
        Task UpdateRefreshTokenAsync(RefreshToken RefreshTokens);
        Task DeleteRefreshTokenAsync(int id);
    }
}

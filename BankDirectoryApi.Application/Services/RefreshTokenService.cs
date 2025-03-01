using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankDirectoryApi.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, UserManager<IdentityUser> userManager)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
        }
        public async Task<IEnumerable<RefreshToken>> GetAllRefreshTokensAsync()
        {
            return await _refreshTokenRepository.GetAllAsync();
        }
        public async Task<RefreshToken?> GetRefreshTokenByIdAsync(int id)
        {
            return await _refreshTokenRepository.GetByIdAsync(id);
        }

    

        public async Task AddRefreshTokenAsync(RefreshToken RefreshToken)
        {
            var user = await _userManager.FindByIdAsync(RefreshToken.UserId);

            if (user == null)
            {
                throw new Exception($"User with ID {RefreshToken.UserId} not found.");
            }
            RefreshToken.User = null;

            await _refreshTokenRepository.AddAsync(RefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken RefreshToken)
        {
            _refreshTokenRepository.Update(RefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task DeleteRefreshTokenAsync(int id)
        {
            var RefreshToken = await _refreshTokenRepository.GetByIdAsync(id);
            if (RefreshToken != null)
            {
                _refreshTokenRepository.Delete(RefreshToken);
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }
    }
}

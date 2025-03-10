using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities; // Assuming your IdentityUser entity is here
using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Common.Services; // Your JWT settings

namespace YourProject.Infrastructure.Identity
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public JwtService(UserManager<IdentityUser> userManager, 
            IConfiguration configuration,IRefreshTokenRepository refreshTokenRepository
            ,IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<string?> GenerateJwtTokenAsync(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration);
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration);
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration);
            var jwtExpirationHours = JwtHelper.GetJwtExpirationHours(_configuration);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = _dateTimeProvider.UtcNow.AddHours(jwtExpirationHours);

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string?> GenerateJwtRefreshTokenAsync(IdentityUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();  // Simple refresh token generation logic
            //todo: Implement a more secure refresh token generation logic
            //todo: Store the refresh token in a secure place (e.g., database)
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                CreationDate = _dateTimeProvider.UtcNow,
                ExpirationDate= _dateTimeProvider.UtcNow.AddMonths(1),
                IsUsed = false,
                IsInvalidated = false,
                IsRevoked = false,

            });
            return refreshToken;
        }
        public async Task<string?> GenerateJwtTokenFromRefreshTokenAsync(string refreshToken)
        {
            var storedRefreshToken = await _refreshTokenRepository.FindAsync(rt => 
            rt.Token == refreshToken);
            var refreshTokenEntity = storedRefreshToken.FirstOrDefault();
            if (refreshTokenEntity == null) return null;
            if (refreshTokenEntity.IsRevoked || refreshTokenEntity.IsInvalidated || refreshTokenEntity.IsUsed) return null;
            if (refreshTokenEntity.ExpirationDate < _dateTimeProvider.UtcNow) return null;
            var user = await _userManager.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null) return null;
            return await GenerateJwtTokenAsync(user);
        }
        public async Task<string?> InvalidateRefreshTokenAsync(string refreshToken)// security breach is detected
        {
            var storedRefreshToken = await _refreshTokenRepository.FindAsync(rt => rt.Token == refreshToken);
            var refreshTokenEntity = storedRefreshToken.FirstOrDefault();
            if (refreshTokenEntity == null) return null;
            if (refreshTokenEntity.IsRevoked || refreshTokenEntity.IsInvalidated || refreshTokenEntity.IsUsed) return null;
            if (refreshTokenEntity.ExpirationDate < _dateTimeProvider.UtcNow) return null;
            var user = await _userManager.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null) return null;
            return await GenerateJwtTokenAsync(user);
        }
    }
}
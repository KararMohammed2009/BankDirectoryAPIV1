using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Exceptions;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IHashService _hashService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISessionHandler _sessionHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IHashService hashService, IConfiguration configuration,
            IDateTimeProvider dateTimeProvider,ISessionHandler sessionHandler,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _hashService = hashService;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _sessionHandler = sessionHandler;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public (string RefreshToken, string HashedRefreshToken) GenerateRefreshTokenAsync()
        {
            byte[] randomBytes = new byte[64]; // Adjust length as needed
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var refreshToken = Convert.ToBase64String(randomBytes);
            var hashedRefreshToken = _hashService.GetHash(refreshToken);
            return (refreshToken, hashedRefreshToken);
        }
        public async Task<(RefreshToken refreshTokenEntity, string refreshToken)> GenerateRefreshTokenEntityAsync(
            string userId
           , ClientInfo clientInfo)
        {

            var refreshTokenPair = GenerateRefreshTokenAsync();

            if (string.IsNullOrEmpty(refreshTokenPair.RefreshToken)
                || string.IsNullOrEmpty(refreshTokenPair.HashedRefreshToken))
                throw new TokenHandlingException("Error in generating valid refresh token");

            var refreshTokenLifetimeDays = _configuration["JwtSettings:RefreshTokenLifetimeDays"];
            if (string.IsNullOrEmpty(refreshTokenLifetimeDays))
                throw new ConfigurationException(
                "Cannot read refreshTokenLifetimeDays from appsetting.json >>> JwtSettings:RefreshTokenLifetimeDays");
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenPair.HashedRefreshToken,
                UserId = userId,
                ExpirationDate = _dateTimeProvider.UtcNow.AddDays(double.Parse(refreshTokenLifetimeDays)),
                CreationDate = _dateTimeProvider.UtcNow,
                IsRevoked = false,
                IsUsed = false,
                SessionId = _sessionHandler.GenerateNewSessionIdAsync(),
                UserAgent = clientInfo.UserAgent,
                CreatedByIp = clientInfo.IpAddress,
            };


            return (refreshTokenEntity, refreshTokenPair.RefreshToken);
        }
        public async Task<RefreshToken> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
            return refreshToken;
        }
        public async Task<bool> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken)
        {
            return await _refreshTokenRepository.RotateRefreshTokenAsync(
                _hashService.GetHash(oldRefreshToken)
                , newRefreshToken);

        }
        public async Task<bool> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? ipAddress)
        {
            return await _refreshTokenRepository.RevokeAllRefreshTokensAsync(userId,sessionId,ipAddress);

        }
    }
}

using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Exceptions;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IHashService _hashService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISessionService _sessionHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IHashService hashService, IConfiguration configuration,
            IDateTimeProvider dateTimeProvider,ISessionService sessionHandler,
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
            try
            {
                byte[] randomBytes = new byte[64]; // Adjust length as needed
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }
                var refreshToken = Convert.ToBase64String(randomBytes);
                var hashedRefreshToken = _hashService.GetHash(refreshToken);
                if (hashedRefreshToken == null || refreshToken ==null) {
                    throw new Exception();
                }
                return (refreshToken, hashedRefreshToken);
            }
            catch(HashServiceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RefreshTokenServiceException("Error in generating refresh token", ex);
            }
        }
        public async Task<(RefreshToken refreshTokenEntity, string refreshToken)> GenerateRefreshTokenEntityAsync(
            string userId
           , ClientInfo clientInfo)
        {
            try
            {
                var refreshTokenPair = GenerateRefreshTokenAsync();


                var refreshTokenLifetimeDays = _configuration["JwtSettings:RefreshTokenLifetimeDays"];
                if (string.IsNullOrEmpty(refreshTokenLifetimeDays))
                    throw new Exception(
                    "configration[JwtSettings:RefreshTokenLifetimeDays] is null");
                var dateNow = _dateTimeProvider.UtcNow;
                var refreshTokenEntity = new RefreshToken
                {
                    TokenHash = refreshTokenPair.HashedRefreshToken,
                    UserId = userId,
                    ExpirationDate = dateNow.AddDays(double.Parse(refreshTokenLifetimeDays)),
                    CreationDate = dateNow,
                    IsRevoked = false,
                    IsUsed = false,
                    SessionId = _sessionHandler.GenerateNewSessionIdAsync(),
                    UserAgent = clientInfo?.UserAgent ?? string.Empty,
                    CreatedByIp = clientInfo?.IpAddress ?? string.Empty,
                };

                if(refreshTokenEntity == null)
                {
                    throw new Exception("Refresh token entity is null");
                }
              
                return (refreshTokenEntity, refreshTokenPair.RefreshToken);
            }
            catch (Exception ex)
            {
                throw new RefreshTokenServiceException("Error in generating refresh token entity", ex);
            }
        }
        public async Task<RefreshToken> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null) throw new RefreshTokenServiceException("RefreshToken is null");
            await _refreshTokenRepository.AddAsync(refreshToken);
            return refreshToken;
        }
        public async Task<bool> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken)
        {
            if(oldRefreshToken == null)
            {
                throw new RefreshTokenServiceException("Old refresh token is null");
            }
            if (newRefreshToken == null)
            {
                throw new RefreshTokenServiceException("New refresh token is null");
            }
            return await _refreshTokenRepository.RotateRefreshTokenAsync(
                _hashService.GetHash(oldRefreshToken)
                , newRefreshToken);

        }
        public async Task<bool> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? ipAddress)
        {
            if(userId == null)
            {
                throw new RefreshTokenServiceException("User id is null");
            }
            if (sessionId == null)
            {
                throw new RefreshTokenServiceException("Session id is null");
            }
          
            return await _refreshTokenRepository.RevokeAllRefreshTokensAsync(userId,sessionId,ipAddress);

        }
    }
}

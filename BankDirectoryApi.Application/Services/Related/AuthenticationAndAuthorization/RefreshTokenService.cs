using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IHashService _hashService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISessionService _sessionHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomNumberProvider _randomNumberProvider;
        public RefreshTokenService(IHashService hashService, IConfiguration configuration,
            IDateTimeProvider dateTimeProvider,ISessionService sessionHandler,
            IRefreshTokenRepository refreshTokenRepository,
            IRandomNumberProvider randomNumberProvider)
        {
            _hashService = hashService;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _sessionHandler = sessionHandler;
            _refreshTokenRepository = refreshTokenRepository;
            _randomNumberProvider = randomNumberProvider;
        }
        public Result<(string RefreshToken, string HashedRefreshToken)> GenerateRefreshTokenAsync()
        {
           
          var randomNumberResult = _randomNumberProvider.GetBase64RandomNumber(64);
            if (randomNumberResult.IsFailed)
            {
                return Result.Fail(new Error("Generate RefreshToken failed")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(randomNumberResult.Errors);
            }
            var refreshToken = randomNumberResult.Value;
            var hashedRefreshTokenResult = _hashService.GetHash(refreshToken);
            if (hashedRefreshTokenResult.IsFailed)
            {
             return Result.Fail(new Error("Generate RefreshToken failed")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(hashedRefreshTokenResult.Errors);
            }
            return Result.Ok((refreshToken, hashedRefreshTokenResult.Value));
        }
        public async Task<Result<(RefreshToken refreshTokenEntity, string refreshToken)>>
            GenerateRefreshTokenEntityAsync(string userId, ClientInfo clientInfo)
        {
           if(string.IsNullOrEmpty(userId))
                return Result.Fail(new Error("Generate RefreshToken Entity failed : userId is null")
                    .WithMetadata("StatusCode",HttpStatusCode.BadRequest));

             var refreshTokenPairResult = GenerateRefreshTokenAsync();
            if (refreshTokenPairResult.IsFailed)
                return Result.Fail(refreshTokenPairResult.Errors);

            var refreshTokenLifetimeDays = _configuration["JwtSettings:RefreshTokenLifetimeDays"];

          

            if (string.IsNullOrEmpty(refreshTokenLifetimeDays))
               return Result.Fail(new Error("Generate RefreshToken Entity failed : Refresh token lifetime days is null")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            

            var dateNowResult = _dateTimeProvider.UtcNow;
                var refreshTokenEntity = new RefreshToken
                {
                    TokenHash = refreshTokenPairResult.Value.HashedRefreshToken,
                    UserId = userId,
                    ExpirationDate = dateNowResult.Value.AddDays(double.Parse(refreshTokenLifetimeDays)),
                    CreationDate = dateNowResult.Value,
                    IsRevoked = false,
                    IsUsed = false,
                    SessionId = _sessionHandler.GenerateNewSessionIdAsync().Value,
                    UserAgent = clientInfo?.UserAgent ?? string.Empty,
                    CreatedByIp = clientInfo?.IpAddress ?? string.Empty,
                };
                return Result.Ok((refreshTokenEntity, refreshTokenPairResult.Value.RefreshToken));
            
            
        }
        public async Task<Result<RefreshToken>> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                return Result.Fail(new Error("Store RefreshToken failed : Refresh token is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            await _refreshTokenRepository.AddAsync(refreshToken);
            return  Result.Ok(refreshToken);
        }
        public async Task<Result<RefreshToken>> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken)
        {
            if(string.IsNullOrEmpty(oldRefreshToken))
            {
             return Result.Fail(new Error("Rotate RefreshToken failed : Old refresh token is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            if (newRefreshToken == null)
            {
               return Result.Fail(new Error("Rotate RefreshToken failed : New refresh token is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }


            var hashedOldRefreshTokenResult = _hashService.GetHash(oldRefreshToken);
            if (!hashedOldRefreshTokenResult.IsSuccess)
            {
                return Result.Fail(new Error("Rotate RefreshToken failed : Hashing old refresh token failed")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(hashedOldRefreshTokenResult.Errors);
            }
            await _refreshTokenRepository.RotateRefreshTokenAsync(
                hashedOldRefreshTokenResult.Value, newRefreshToken);

                return Result.Ok(newRefreshToken);

        }
        public async Task<Result<string>> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? ipAddress)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return Result.Fail(new Error("Revoke All Refresh Tokens failed : userId is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            if (string.IsNullOrEmpty(sessionId))
            {
               return Result.Fail(new Error("Revoke All Refresh Tokens failed : sessionId is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
          
             await _refreshTokenRepository.RevokeAllRefreshTokensAsync(userId,sessionId,ipAddress);
            return Result.Ok(userId);

        }
    }
}

using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to handle refresh tokens
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IHashService _hashService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISessionService _sessionHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomNumberProvider _randomNumberProvider;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hashService"></param>
        /// <param name="configuration"></param>
        /// <param name="dateTimeProvider"></param>
        /// <param name="sessionHandler"></param>
        /// <param name="refreshTokenRepository"></param>
        /// <param name="randomNumberProvider"></param>
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
        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <returns>The refresh token and its hash</returns>
        public Result<(string RefreshToken, string HashedRefreshToken)> GenerateRefreshToken()
        {
           
          var randomNumberResult = _randomNumberProvider.GetBase64RandomNumber(64);
            if (randomNumberResult.IsFailed)
                return randomNumberResult.ToResult<(string RefreshToken, string HashedRefreshToken)>();
            var refreshToken = randomNumberResult.Value;
            var hashedRefreshTokenResult = _hashService.GetHash(refreshToken);
            return Result.Ok((refreshToken, hashedRefreshTokenResult.Value));
        }
        /// <summary>
        /// Generate a new refresh token entity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The refresh token entity and the refresh token</returns>
        public  Result<(RefreshToken refreshTokenEntity, string refreshToken)>
            GenerateRefreshTokenEntity(string userId, ClientInfo clientInfo)
        {
          var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<(RefreshToken refreshTokenEntity, string refreshToken)>();

            var refreshTokenPairResult = GenerateRefreshToken();
            if (refreshTokenPairResult.IsFailed)
                return refreshTokenPairResult.ToResult<(RefreshToken refreshTokenEntity, string refreshToken)>();

            var refreshTokenLifetimeDays = _configuration["JwtSettings:RefreshTokenLifetimeDays"];


            int refreshTokenLifetimeDaysValue;
            int.TryParse(refreshTokenLifetimeDays, out refreshTokenLifetimeDaysValue);
            if (refreshTokenLifetimeDaysValue <= 0)
                return Result.Fail(new Error("Generate RefreshToken Entity failed : Refresh token lifetime days read from Configration is invalid or less than 0")
                     .WithMetadata("ErrorCode", CommonErrors.ConfigurationError));
           


            var dateNowResult = _dateTimeProvider.UtcNow;
                var refreshTokenEntity = new RefreshToken
                {
                    TokenHash = refreshTokenPairResult.Value.HashedRefreshToken,
                    UserId = userId,
                    ExpirationDate = dateNowResult.Value.AddDays(refreshTokenLifetimeDaysValue),
                    CreationDate = dateNowResult.Value,
                    IsRevoked = false,
                    IsUsed = false,
                    SessionId = _sessionHandler.GenerateNewSessionIdAsync().Value,
                    UserAgent = clientInfo?.UserAgent ?? string.Empty,
                    CreatedByIp = clientInfo?.IpAddress ?? string.Empty,
                };
                return Result.Ok((refreshTokenEntity, refreshTokenPairResult.Value.RefreshToken));
            
            
        }
        /// <summary>
        /// Store the refresh token in its repository
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>The stored refresh token entity</returns>
        public async Task<Result<RefreshToken>> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            var validationResult = ValidationHelper.ValidateNullModel(refreshToken, "refreshToken");
            if (validationResult.IsFailed)
                return validationResult.ToResult<RefreshToken>();

            await _refreshTokenRepository.AddAsync(refreshToken);
            var result = await _refreshTokenRepository.SaveChangesReturnStatusAsync();
            if (result == 0)
                return Result.Fail(new Error("Store RefreshToken failed : No refresh token was stored")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            if (result < 0)
                return Result.Fail(new Error("Store RefreshToken failed : An error occurred while storing refresh token")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            return  Result.Ok(refreshToken);
        }
        /// <summary>
        /// Rotate the refresh token
        /// </summary>
        /// <param name="oldRefreshToken"></param>
        /// <param name="newRefreshToken"></param>
        /// <returns>The rotated new refresh token entity</returns>
        public async Task<Result<RefreshToken>> RotateRefreshTokenAsync(string oldRefreshToken, RefreshToken newRefreshToken)
        {
           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(oldRefreshToken, "oldRefreshToken");
            if (validationResult.IsFailed)
                return validationResult.ToResult<RefreshToken>();
            validationResult = ValidationHelper.ValidateNullModel(newRefreshToken, "newRefreshToken");
            if (validationResult.IsFailed)
                return validationResult.ToResult<RefreshToken>();
         


            var hashedOldRefreshTokenResult = _hashService.GetHash(oldRefreshToken);
            if (hashedOldRefreshTokenResult.IsFailed)
                return hashedOldRefreshTokenResult.ToResult<RefreshToken>();
            var rotateResult = await _refreshTokenRepository.RotateRefreshTokenAsync(
                hashedOldRefreshTokenResult.Value, newRefreshToken);
            if (rotateResult.IsFailed)
                return rotateResult.ToResult<RefreshToken>();
            var result = await _refreshTokenRepository.SaveChangesReturnStatusAsync();
            if (result == 0)
                return Result.Fail(new Error("Rotate RefreshToken failed : No refresh token was rotated")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            if (result < 0)
                return Result.Fail(new Error("Rotate RefreshToken failed : An error occurred while rotating refresh token")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            
            return Result.Ok(newRefreshToken);

        }
        /// <summary>
        /// Revoke a refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="ipAddress"></param>
        /// <returns>The value of user id</returns>
        public async Task<Result<string>> RevokeAllRefreshTokensAsync(string userId, string sessionId, string? ipAddress)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(sessionId, "sessionId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();


            var revokeResult = await _refreshTokenRepository.RevokeAllRefreshTokensAsync(userId, sessionId, ipAddress);
            if (revokeResult.IsFailed)
                return revokeResult.ToResult<string>();
            var result = await _refreshTokenRepository.SaveChangesReturnStatusAsync();
            if (result == 0)
                return Result.Fail(new Error("Revoke RefreshToken failed : No refresh token was revoked")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            if (result < 0)
                return Result.Fail(new Error("Revoke RefreshToken failed : An error occurred while revoking refresh token")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            return Result.Ok(userId);

        }
        /// <summary>
        /// Revoke a refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="ipAddress"></param>
        /// <returns>The value of user id</returns>
        public async Task<Result<string>> RevokeAllRefreshTokensAsync(string userId, string? ipAddress)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
           


            var revokeResult = await _refreshTokenRepository.RevokeAllRefreshTokensAsync(userId, ipAddress);
            if (revokeResult.IsFailed)
                return revokeResult.ToResult<string>();
            var result = await _refreshTokenRepository.SaveChangesReturnStatusAsync();
            if (result == 0)
                return Result.Fail(new Error("Revoke RefreshToken failed : No refresh token was revoked")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            if (result < 0)
                return Result.Fail(new Error("Revoke RefreshToken failed : An error occurred while revoking refresh token")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            return Result.Ok(userId);

        }
    }
}

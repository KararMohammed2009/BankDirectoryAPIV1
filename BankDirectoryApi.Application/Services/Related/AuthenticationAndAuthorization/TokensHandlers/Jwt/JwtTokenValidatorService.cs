using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using FluentResults;
using System.Net;
using BankDirectoryApi.Common.Extensions;
using Microsoft.Extensions.Logging;
using BankDirectoryApi.Common.Errors;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    /// <summary>
    /// Service to validate JWT tokens
    /// </summary>
    public class JwtTokenValidatorService : ITokenValidatorService
    {

        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="jwtSecurityTokenHandler"></param>
        /// <param name="logger"></param>
        public JwtTokenValidatorService(
            IConfiguration configuration, JwtSecurityTokenHandler jwtSecurityTokenHandler
            ,ILogger logger)
        {
            _configuration = configuration;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _logger = logger;
        }

        /// <summary>
        /// Validate JWT Access Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>True if the token is valid, false otherwise</returns>
        public async Task<Result<bool>> ValidateAccessTokenAsync(string accessToken)
        {

            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration,_logger);
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration, _logger);
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration, _logger);
           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value));
            

            
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtIssuer.Value,
                    ValidAudience = jwtAudience.Value
                };

                var validatedToken = await _jwtSecurityTokenHandler.ValidateTokenAsync(accessToken, validationParameters);
            if (validatedToken == null)
            {
                _logger.LogError("Validate accessToken failed by JwtSecurityTokenHandler");
                return Result.Fail(new Error("Validate accessToken failed by JwtSecurityTokenHandler")
                    .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
            }
            return Result.Ok(validatedToken.IsValid);
           
            
        }
        
    }
}
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using FluentResults;
using Microsoft.Extensions.Logging;
using BankDirectoryApi.Common.Errors;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    /// <summary>
    /// Service to validate JWT tokens
    /// </summary>
    public class JwtTokenValidatorService : ITokenValidatorService
    {

        private readonly ILogger<JwtTokenValidatorService> _logger;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public JwtTokenValidatorService(
          ILogger<JwtTokenValidatorService> logger, JwtSettings jwtSettings)
        {
           
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Validate JWT Access Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>True if the token is valid, false otherwise</returns>
        public async Task<Result<bool>> ValidateAccessTokenAsync(string accessToken)
        {

           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            

            
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var validatedToken = await jwtSecurityTokenHandler.ValidateTokenAsync(accessToken, validationParameters);
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
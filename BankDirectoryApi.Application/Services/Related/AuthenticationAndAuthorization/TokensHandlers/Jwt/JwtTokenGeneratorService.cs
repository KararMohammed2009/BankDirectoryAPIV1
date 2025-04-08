using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using FluentResults;
using BankDirectoryApi.Common.Errors;
using Microsoft.Extensions.Logging;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    /// <summary>
    /// Service to generate JWT tokens
    /// </summary>
    public class JwtTokenGeneratorService : ITokenGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidProvider _guidProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dateTimeProvider"></param>
        /// <param name="guidProvider"></param>
        /// <param name="logger"></param>
        public JwtTokenGeneratorService(
            IConfiguration configuration
            ,IDateTimeProvider dateTimeProvider,
            IGuidProvider guidProvider,
            ILogger logger)
        {
            
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _guidProvider = guidProvider;
            _logger = logger;
        }

        /// <summary>
        /// Generate JWT Access Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="roles"></param>
        /// <param name="userClaims"></param>
        /// <returns>The value of generated JWT Access Token</returns>
        public Result<string> GenerateAccessToken(string userId, string userName, string email, IEnumerable<string>? roles, Dictionary<string, string>? userClaims)
       
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userName, "userName");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();



            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, _guidProvider.NewGuid().Value.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (userClaims != null)
            {
                foreach (var claim in userClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration,_logger).Value;
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration,_logger).Value;
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration, _logger).Value;
            var jwtExpirationHours = JwtHelper.GetJwtExpirationHours(_configuration, _logger).Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = _dateTimeProvider.UtcNow.Value.AddHours(jwtExpirationHours);

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Result.Ok(jwtToken);
        }


    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using System.Security.Cryptography;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using BankDirectoryApi.Common.Exceptions;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenGeneratorService : ITokenGeneratorService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidProvider _guidProvider;

        public JwtTokenGeneratorService(
            IConfiguration configuration
            ,IDateTimeProvider dateTimeProvider,
            IGuidProvider guidProvider,
            IUserService userService)
        {
            
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _guidProvider = guidProvider;
            _userService = userService;
        }

        public async Task<string> GenerateAccessTokenAsync(string userId
            ,string userName , string email, IEnumerable<string>? roles,
            Dictionary<string,string>? userClaims)
        {
            try
            {
                if (userId == null) throw new Exception("userId is null");
                if (userName == null) throw new Exception("userName is null");
                if (email == null) throw new Exception("email is null");

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, _guidProvider.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email)
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
                        claims.Add(new Claim(claim.Key,claim.Value));
                    }
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
            catch (Exception ex)
            {
                throw new JwtTokenGeneratorServiceException("Generate Jwt AccessToken Failed", ex);
            }
        }
       
        
    }
}
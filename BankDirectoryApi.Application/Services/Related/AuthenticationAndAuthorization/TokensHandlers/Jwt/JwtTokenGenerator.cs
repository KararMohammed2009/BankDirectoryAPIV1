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

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;

        public JwtTokenGenerator(UserManager<IdentityUser> userManager, 
            IConfiguration configuration
            ,IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<string> GenerateAccessTokenAsync(IdentityUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName)
                    || string.IsNullOrEmpty(user.Email))
            {
                throw new TokenHandlingException("Invalid User or null");
            }
            try
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
            catch (Exception ex)
            {
                throw new TokenHandlingException("Generate Jwt AccessToken Failed",ex);
            }
        }
       
        
    }
}
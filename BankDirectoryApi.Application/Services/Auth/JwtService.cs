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
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Application.Interfaces.Auth;
using System.Security.Cryptography;
using BCrypt.Net;

namespace BankDirectoryApi.Application.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
       
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHashService _hashService;

        public JwtService(UserManager<IdentityUser> userManager, 
            IConfiguration configuration
            ,IDateTimeProvider dateTimeProvider,IHashService hashService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _hashService = hashService;
        }

        public async Task<string?> GenerateAccessTokenAsync(IdentityUser user)
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
        public async Task<(string? RefreshToken,string? HashedRefreshToken)> 
            GenerateRefreshTokenAsync(IdentityUser user)
        {
            byte[] randomBytes = new byte[64]; // Adjust length as needed
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var refreshToken =  Convert.ToBase64String(randomBytes);
            var hashedRefreshToken = _hashService.GetHash(refreshToken);
            return (refreshToken,hashedRefreshToken);
        }
      
        public async Task<bool?> ValidateAccessTokenAsync(string accessToken)
        {
            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration);
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration);
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience
                };

                var validatedToken = await tokenHandler.ValidateTokenAsync(accessToken, validationParameters);

                return validatedToken.IsValid;
            }
            catch (SecurityTokenException ex) // Catch specific JWT exceptions
            {
                // Log the exception details
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Token validation failed due to argument exception: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occured during token validation: {ex.Message}");
                return false;
            }
        }
        public string GenerateNewSessionIdAsync()
        {
            byte[] randomBytes = new byte[64]; // Adjust length as needed
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);

        }
    }
}
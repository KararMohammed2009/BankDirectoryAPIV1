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

        public async Task<string> GenerateAccessTokenAsync(UserDTO user)
        {
            try
            {

                if (user == null || string.IsNullOrEmpty(user.UserName)
                    || string.IsNullOrEmpty(user.Email))
            {
                throw new NotFoundException();
            }
              
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, _guidProvider.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

                foreach (var role in user.Roles)
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
            catch (NotFoundException ex)
            {
                throw new JwtTokenGeneratorServiceException("Invalid User",ex);
            }
            catch (Exception ex)
            {
                throw new JwtTokenGeneratorServiceException("Generate Jwt AccessToken Failed", ex);
            }
        }
       
        
    }
}
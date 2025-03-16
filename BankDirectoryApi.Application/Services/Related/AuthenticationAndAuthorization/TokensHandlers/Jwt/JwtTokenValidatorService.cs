using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenValidatorService : ITokenValidatorService
    {

        private readonly IConfiguration _configuration;
       

        public JwtTokenValidatorService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

      
        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            try
            {
                var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration);
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration);
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            
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
            catch (Exception ex) 
            {
                throw new JwtTokenValidatorServiceException("Validate Jwt AccessToken Failed", ex);
            }
            
        }
        
    }
}
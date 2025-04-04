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

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenValidatorService : ITokenValidatorService
    {

        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtTokenValidatorService(
            IConfiguration configuration, JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _configuration = configuration;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }


        public async Task<Result<bool>> ValidateAccessTokenAsync(string accessToken)
        {

            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration);
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration);
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration);
            if (!jwtSecret.IsSuccess || !jwtIssuer.IsSuccess || jwtAudience.IsSuccess)
            {
                return Result.Fail(new Error("").WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(jwtSecret.Errors)
                    .WithErrors(jwtIssuer.Errors)
                    .WithErrors(jwtAudience.Errors);
            }
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
                return Result.Fail(new Error("Validate accessToken failed by JwtSecurityTokenHandler")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
                return Result.Ok(validatedToken.IsValid);
           
            
        }
        
    }
}
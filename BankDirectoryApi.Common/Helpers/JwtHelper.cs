using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace BankDirectoryApi.Common.Helpers
{
    public static class JwtHelper
    {
        public static Result<string> GetJwtSecretKey(IConfiguration configuration)
        {
            
                var secretKey = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_SECRET")
                                ?? configuration["JwtSettings:SecretKey"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT Secret Key is not configured!");
                }

                return Result.Ok(secretKey);
            
        }
        public static Result<string> GetJwtIssuer(IConfiguration configuration)
        {
            var issuer = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_ISSUER")
                            ?? configuration["JwtSettings:Issuer"];

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured!");
            }

            return Result.Ok(issuer);
        }
        public static Result<string> GetJwtAudience(IConfiguration configuration)
        {
            var audience = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_AUDIENCE")
                            ?? configuration["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured!");
            }

            return Result.Ok(audience);
        }
        public static Result<int> GetJwtExpirationHours(IConfiguration configuration)
        {
            var expirationHoursString = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_EXPIRATIONHOURS")
                            ?? configuration["JwtSettings:ExpirationHours"];

            if (string.IsNullOrEmpty(expirationHoursString))
            {
                throw new InvalidOperationException("JWT ExpirationHours is not configured!");
            }
            int expirationHours;
            if(!int.TryParse(expirationHoursString, out expirationHours))
            {
                return Result.Fail(new Error("Error in Parsing JWT ExpirationHours").WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            }
            
            return Result.Ok(expirationHours);
        }
    }
}

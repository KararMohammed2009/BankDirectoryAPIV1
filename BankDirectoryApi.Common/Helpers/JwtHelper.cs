using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BankDirectoryApi.Common.Helpers
{
    public static class JwtHelper
    {
        public static string GetJwtSecretKey(IConfiguration configuration)
        {
            var secretKey = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_SECRET")
                            ?? configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured!");
            }

            return secretKey;
        }
        public static string GetJwtIssuer(IConfiguration configuration)
        {
            var issuer = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_ISSUER")
                            ?? configuration["JwtSettings:Issuer"];

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured!");
            }

            return issuer;
        }
        public static string GetJwtAudience(IConfiguration configuration)
        {
            var audience = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_AUDIENCE")
                            ?? configuration["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured!");
            }

            return audience;
        }
        public static int GetJwtExpirationHours(IConfiguration configuration)
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
                expirationHours = 0;
            }
            
            return expirationHours;
        }
    }
}

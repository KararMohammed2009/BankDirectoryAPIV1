using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Common.Errors;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankDirectoryApi.Common.Helpers
{
    /// <summary>
    /// Helper class for JWT token generation and validation
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Get JWT Secret Key from environment variable or configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns> The value of the JWT Secret Key</returns>
        public static Result<string> GetJwtSecretKey(IConfiguration configuration, ILogger logger)
        {
            string? secretKey = null; 
            
                secretKey = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_SECRET");
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                logger.LogWarning("Error getting (BankDirectoryApi_JWT_SECRET) from environment variable");

                secretKey = configuration["JwtSettings:SecretKey"];
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    logger.LogCritical("Error getting (JwtSettings:SecretKey) from configuration");

                    return Result.Fail(new Error("Error getting JWT Secret Key from from environment variable nither configuration")
                        .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
                }

            }

            return Result.Ok(secretKey);
        }
        /// <summary>
        /// Get JWT Issuer from environment variable or configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns> The value of the JWT Issuer</returns>
        public static Result<string> GetJwtIssuer(IConfiguration configuration,ILogger logger)
        {
            string? issuer = null;

            issuer = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_ISSUER");
            if (string.IsNullOrWhiteSpace(issuer))
            {
                logger.LogWarning("Error getting (BankDirectoryApi_JWT_ISSUER) from environment variable");

                issuer = configuration["JwtSettings:Issuer"];
                if (string.IsNullOrWhiteSpace(issuer))
                {
                    logger.LogCritical("Error getting (JwtSettings:Issuer) from configuration");

                    return Result.Fail(new Error("Error getting JWT Issuer from from environment variable nither configuration")
                        .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
                }

            }

            return Result.Ok(issuer);
          
        }
        /// <summary>
        /// Get JWT Audience from environment variable or configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns> The value of the JWT Audience</returns>
        public static Result<string> GetJwtAudience(IConfiguration configuration,ILogger logger)
        {
            string? audience = null;
            audience = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_AUDIENCE");
            if (string.IsNullOrWhiteSpace(audience))
            {
                logger.LogWarning("Error getting (BankDirectoryApi_JWT_AUDIENCE) from environment variable");
                audience = configuration["JwtSettings:Audience"];
                if (string.IsNullOrWhiteSpace(audience))
                {
                    logger.LogCritical("Error getting (JwtSettings:Audience) from configuration");
                    return Result.Fail(new Error("Error getting JWT Audience from from environment variable nither configuration")
                        .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
                }
            }
            return Result.Ok(audience);
        }
        /// <summary>
        /// Get JWT Expiration Hours from environment variable or configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns> The value of the JWT Expiration Hours</returns>
        public static Result<int> GetJwtExpirationHours(IConfiguration configuration,ILogger logger)
        {
            string? expirationHours = null;
            expirationHours = Environment.GetEnvironmentVariable("BankDirectoryApi_JWT_EXPIRATION_HOURS");
            if (string.IsNullOrWhiteSpace(expirationHours))
            {
                logger.LogWarning("Error getting (BankDirectoryApi_JWT_EXPIRATION_HOURS) from environment variable");
                expirationHours = configuration["JwtSettings:ExpirationHours"];
                if (string.IsNullOrWhiteSpace(expirationHours))
                {
                    logger.LogCritical("Error getting (JwtSettings:ExpirationHours) from configuration");
                    return Result.Fail(new Error("Error getting JWT Expiration Hours from from environment variable nither configuration")
                        .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
                }
            }
            int expirationHoursValue;
            if (!int.TryParse(expirationHours, out expirationHoursValue))
            {
                logger.LogCritical("Error parsing (JwtSettings:ExpirationHours) from configuration");
                return Result.Fail(new Error("Error parsing JWT Expiration Hours from from environment variable nither configuration")
                    .WithMetadata("ErrorCode", CommonErrors.InternalServerError));
            }
            return Result.Ok(expirationHoursValue);

        }
    }
}

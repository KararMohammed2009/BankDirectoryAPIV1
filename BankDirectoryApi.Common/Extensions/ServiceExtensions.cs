using BankDirectoryApi.Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("JWT");



                var jwtSecret = JwtHelper.GetJwtSecretKey(configuration, logger);
                var jwtIssuer = JwtHelper.GetJwtIssuer(configuration, logger);
                var jwtAudience = JwtHelper.GetJwtAudience(configuration, logger);
                var jwtExpirationHours = JwtHelper.GetJwtExpirationHours(configuration, logger);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidIssuer = jwtIssuer.Value,
                        ValidAudience = jwtAudience.Value,
                        ClockSkew = TimeSpan.Zero, // Optional: disables clock skew
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            // Add logging here to inspect the token and validation results
                            logger.LogInformation("Token validated: {Token}", context.SecurityToken);
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context => {
                            logger.LogError("Authentication failed: {Exception}", context.Exception);
                            return Task.CompletedTask;
                        }
                    };
                });

                return services;
            }
        }
    }
}

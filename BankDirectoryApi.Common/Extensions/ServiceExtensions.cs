using BankDirectoryApi.Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecret = JwtHelper.GetJwtSecretKey(configuration);
            var jwtIssuer = JwtHelper.GetJwtIssuer(configuration);
            var jwtAudience = JwtHelper.GetJwtAudience(configuration);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,  // for better security
                        ValidateAudience = true, // for better security
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience
                    };
                });

            return services;
        }
    }
}

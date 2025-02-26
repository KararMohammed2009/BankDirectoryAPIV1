

using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Services;
using BankDirectoryApi.Infrastructure.Data;
using BankDirectoryApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BankDirectoryApi.API.Middleware;
using AspNetCoreRateLimit;
using BankDirectoryApi.Common.Extensions;

namespace BankDirectoryApi.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddTheSwagger(this WebApplicationBuilder builder)
        {

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
        }
        public static void AddTheCors(this WebApplicationBuilder builder)
        {
            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
        }
        public static void AddJwtAuth(this WebApplicationBuilder builder)
        {
            // Add JWT Authentication and use authentication and authorization
            builder.Services.AddJwtAuthentication(builder.Configuration);
        }
        public static void AddLimitRate(this WebApplicationBuilder builder)
        {
            // Register Application RateLimit
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
            builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
            //  Services needed for rate limiting
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

        }
        public static void UseExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();  // Catches unhandled exceptions
        }
        public static void UseRequestLoggingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();  // Log Requests
        }
        public static void UseRateLimitLoggingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<RateLimitLoggingMiddleware>();  // Rate Limit Logging
        }
        public static void UseTheSwagger(this WebApplication app )
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    
        public static void UseJwtAuth(this WebApplication app)
        {
            // Use JWT authentication and authorization
            app.UseMiddleware<JwtClientIdMiddleware>(); // Extracts Client ID from JWT
            app.UseAuthentication();
            app.UseAuthorization();
        }
        public static void UseLimitRate(this WebApplication app)
        {
            // Use Rate Limiting
            app.UseIpRateLimiting();
            app.UseClientRateLimiting();

        }
       

        public static void AddApplicationMappers(this IServiceCollection services)
        {

            // Register Automapper for dtos
            services.AddAutoMapper(typeof(BankProfile));
            services.AddAutoMapper(typeof(BranchProfile));
            services.AddAutoMapper(typeof(BankWithBranchesProfile));

        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register application-layer services
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IATMService, ATMService>();
            services.AddScoped<ICardService, CardService>();
        }


        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories, for example:
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IATMRepository, ATMRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
        }
    }
}

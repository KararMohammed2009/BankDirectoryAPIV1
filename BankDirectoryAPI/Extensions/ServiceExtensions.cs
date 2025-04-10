using BankDirectoryApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Application.Mappings;
using BankDirectoryApi.API.Middleware;
using AspNetCoreRateLimit;
using BankDirectoryApi.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using BankDirectoryApi.Infrastructure.Repositories;
using BankDirectoryApi.Infrastructure.Data;
using FluentValidation;
using BankDirectoryApi.API.Validators;
using Asp.Versioning;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Application.Services.Main;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Application.Services.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.Communications;
using BankDirectoryApi.Application.Services.Related.Communications;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt;
using BankDirectoryApi.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using BankDirectoryApi.API.Validators.Auth;
using Serilog;

namespace BankDirectoryApi.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddTheSerilogLogger(this WebApplicationBuilder builder)
        {
                    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

                    builder.Host.UseSerilog();
        }
        public static void AddTheVersioning(this WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(options =>
            {
                // Automatically assume default version if unspecified in requests
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Set the default version to 1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Allow reporting the available API versions to clients via the response headers
                options.ReportApiVersions = true;

                // Read the version from the URL path (e.g., /api/v1/)
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
        }
        public static void AddTheValidators(this WebApplicationBuilder builder)
        {
            // Register FluentValidation validators

            #region Auth
            builder.Services.AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LogoutUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
            #endregion
            builder.Services.AddValidatorsFromAssemblyContaining<BankDTOValidator>();



        }
        public static void AddTheAuthentication(this WebApplicationBuilder builder)
        {
           

            // Configure authentication for external providers
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
                })
                .AddTwitter(options =>
                {
                    options.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"];
                });
        }
        public static void AddTheAuthorization(this WebApplicationBuilder builder)
        {
           builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAccessSomething", policy =>
                    policy.RequireClaim("CanAccessSomething", "true")); 
            });


        }

        public static void AddTheUserServices(this WebApplicationBuilder builder)
        {
            // Register User Services
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
            builder.Services.AddScoped<IHashService, HashService>();
            builder.Services.AddScoped<ITokenGeneratorService, JwtTokenGeneratorService>();
            builder.Services.AddScoped<ITokenValidatorService, JwtTokenValidatorService>();
            builder.Services.AddScoped<ITokenParserService, JwtTokenParserService>();
            builder.Services.AddScoped<JwtSecurityTokenHandler>();


       

            // Register External Authentication Providers
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<IExternalAuthProviderService, GoogleAuthProviderService>();
            builder.Services.AddScoped<IExternalAuthProviderServiceFactory, ExternalAuthProviderFactory>();
            builder.Services.AddScoped<GoogleAuthProviderService>();


            builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Registers Identity services (User + Role management)
            .AddEntityFrameworkStores<ApplicationDbContext>() //Configures Identity to use Entity Framework Core with ApplicationDbContext
            .AddDefaultTokenProviders(); //Enables password reset, email confirmation, 2FA tokens
                                         // Register ASP.NET Identity Managers
            builder.Services.AddScoped<UserManager<IdentityUser>>();
            builder.Services.AddScoped<SignInManager<IdentityUser>>();
            builder.Services.AddScoped<RoleManager<IdentityRole>>();

        }
      
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
        public static void UseTheCors(this WebApplication app)
        {
            app.UseCors("AllowAll");
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
            app.UseMiddleware<ClientParametersExtractorMiddleware>(); // Extracts Client ID from JWT
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
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            // Register Common Services
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IGuidProvider, GuidProvider>();
            services.AddHttpContextAccessor();  
        }


        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            services.AddDbContext<MyIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories, for example:
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IATMRepository, ATMRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
    }
}

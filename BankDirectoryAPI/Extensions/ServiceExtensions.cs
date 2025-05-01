using BankDirectoryApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Application.Mappings;
using BankDirectoryApi.API.Middleware;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Identity;
using BankDirectoryApi.Infrastructure.Repositories;
using BankDirectoryApi.Infrastructure.Data;
using FluentValidation;
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

using Microsoft.OpenApi.Models;
using Serilog;
using BankDirectoryApi.Domain.Entities.Identity;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.API.Mappings.Classes;
using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification;
using BankDirectoryApi.Common.Helpers;
using Twilio.Clients;
using SendGrid;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BankDirectoryApi.Infrastructure.Services.ThirdParties;
using BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification;
using BankDirectoryApi.API.Validators.Related.Auth;
using BankDirectoryApi.API.Validators.Core.Bank;
using BankDirectoryApi.API.Validators.Core.Branch;
using BankDirectoryApi.API.Validators.Core.ATM;
using BankDirectoryApi.API.Validators.Core.Card;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using BankDirectoryApi.API.Models;
using System.Net;

namespace BankDirectoryApi.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddGlobalMappers(this IServiceCollection services)
        {
            services.AddScoped<IActionGlobalMapper,ActionGlobalMapperV1>();
        }
        public static void AddTheSerilogLogger(this WebApplicationBuilder builder)
        {
             Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day )
            .CreateLogger();

                  
            builder.Host.UseSerilog((context, services, configuration) =>
             {
                 configuration
                     .ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services);
             });
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
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                               .Where(x => x.Value?.Errors.Any() == true)
                               .Select(x => new ApiError
                               {
                                   Field = x.Key,
                                   Message = x.Value?.Errors.First().ErrorMessage ?? "Unknown error",
                               })
                               .ToList();

                    return new BadRequestObjectResult(new
                         ApiResponse<object>(errors, null, (int)HttpStatusCode.BadRequest));
                };
            });
            #region Related  
            builder.Services.AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LogoutUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserByAdminDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<EmailConfirmationDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<PaginationInfoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RoleDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserFilterDtoValidator>();
            #endregion

            #region Core  
            builder.Services.AddValidatorsFromAssemblyContaining<BankDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<BankUpdateDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<BankFilterDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<BranchDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<BranchUpdateDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<BranchFilterDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<ATMDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ATMUpdateDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ATMFilterDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<CardDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CardUpdateDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CardFilterDTOValidator>();
            #endregion


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
        public static void AddTheExternalServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ITwilioRestClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<ITwilioRestClient>>();

                var accountSid = SecureVariablesHelper.GetSecureVariable("Sms_Twilio_AccountSid", configuration, "Sms:Twilio:AccountSid", logger).Value;
                var authToken = SecureVariablesHelper.GetSecureVariable("Sms_Twilio_AuthToken", configuration, "Sms:Twilio:AuthToken", logger).Value;

                return new TwilioRestClient(accountSid, authToken);
            });
            builder.Services.AddSingleton<ISendGridClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<ISendGridClient>>();

                var accountSid = SecureVariablesHelper.GetSecureVariable("Email_SendGrid_ApiKey", configuration, "Email:SendGrid:ApiKey", logger).Value;

                return new SendGridClient(accountSid);
            });
            builder.Services.AddSingleton<TwilioSettings>(provider => {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<TwilioSettings>>();
                return new TwilioSettings
                {
                    FromEmail = SecureVariablesHelper.GetSecureVariable("Email_SendGrid_FromEmail", configuration, "Email:SendGrid:FromEmail", logger).Value,
                    FromName = SecureVariablesHelper.GetSecureVariable("Email_SendGrid_FromName", configuration, "Email:SendGrid:FromName", logger).Value,
                    FromPhoneNumber = SecureVariablesHelper.GetSecureVariable("Sms_Twilio_FromNumber", configuration, "Sms:Twilio:FromNumber", logger).Value,
                    VerificationServiceSid = SecureVariablesHelper.GetSecureVariable("Verification_Twilio_ServiceSid", configuration, "Verification:Twilio:ServiceSid", logger).Value,
                };
            });
            builder.Services.AddScoped<ISmsService, TwilioSmsService>();
            builder.Services.AddScoped<IEmailService, TwilioEmailService>();
            builder.Services.AddScoped<ISmsVerificationService, TwilioSmsVerificationService>();
            builder.Services.AddScoped<IEmailVerificationService, TwilioEmailVerificationService>();
            


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
            
            builder.Services.AddScoped<ITokenGeneratorService, JwtTokenGeneratorService>();
            builder.Services.AddScoped<ITokenValidatorService, JwtTokenValidatorService>();
            builder.Services.AddScoped<ITokenParserService, JwtTokenParserService>();
           



            // Register External Authentication Providers
            builder.Services.AddHttpClient<GoogleAuthProviderService>();
            builder.Services.AddScoped<IExternalAuthProviderServiceFactory, ExternalAuthProviderFactory>();

            
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>() // Registers Identity services (User + Role management)
            .AddEntityFrameworkStores<ApplicationDbContext>() //Configures Identity to use Entity Framework Core with ApplicationDbContext
            .AddDefaultTokenProviders(); //Enables password reset, email confirmation, 2FA tokens
                                         // Register ASP.NET Identity Managers
            builder.Services.AddScoped<UserManager<ApplicationUser>>();
            builder.Services.AddScoped<SignInManager<ApplicationUser>>();
            builder.Services.AddScoped<RoleManager<ApplicationRole>>();
            builder.Services.AddScoped<DbInitializer>();

            
        }
      
        public static void AddTheSwagger(this WebApplicationBuilder builder)
        {
            var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled");
            if (builder.Environment.IsDevelopment() || swaggerEnabled)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    // JWT Bearer token authentication
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter JWT with Bearer into field",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankDirectoryApi", Version = "v1" });
                    c.EnableAnnotations();
                    c.SupportNonNullableReferenceTypes();
                }
            );
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
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<JwtSettings>>();
            var jwtSettings = new JwtSettings
            {
                SecretKey = SecureVariablesHelper.GetSecureVariable("BankDirectoryApi_JWT_SECRET", configuration, "JwtSettings:SecretKey", logger).Value,
                Issuer = SecureVariablesHelper.GetSecureVariable("BankDirectoryApi_JWT_ISSUER", configuration, "JwtSettings:Issuer", logger).Value,
                Audience = SecureVariablesHelper.GetSecureVariable("BankDirectoryApi_JWT_AUDIENCE", configuration, "JwtSettings:Audience", logger).Value,
                AccessTokenExpirationHours = int.Parse(SecureVariablesHelper.GetSecureVariable("BankDirectoryApi_JWT_EXPIRATION_HOURS", configuration, "JwtSettings:ExpirationHours", logger).Value),
            };
            builder.Services.AddSingleton<JwtSettings>(provider =>
            {
                //var configuration = provider.GetRequiredService<IConfiguration>();
                //var logger = provider.GetRequiredService<ILogger<JwtSettings>>();
                return jwtSettings;
            });
            // Add JWT Authentication and use authentication and authorization
         
              
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

                builder.Services.AddAuthentication(options =>
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
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
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
                        OnAuthenticationFailed = context =>
                        {
                            logger.LogError("Authentication failed: {Exception}", context.Exception);
                            return Task.CompletedTask;
                        }
                    };
                });
            }
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
            var swaggerEnabled = app.Configuration.GetValue<bool>("Swagger:Enabled");
            if (app.Environment.IsDevelopment() || swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Title v1");
                });
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
            services.AddAutoMapper(typeof(BankWithBranchesProfile));
            services.AddAutoMapper(typeof(BankWithATMsProfile));
            services.AddAutoMapper(typeof(BankWithCardsProfile));

        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register application-layer services
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IATMService, ATMService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            // Register Common Services
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IGuidProvider, GuidProvider>();
            services.AddScoped<IRandomNumberProvider, RandomNumberProvider>();
            services.AddScoped<IHashService, HashService>();
            services.AddHttpContextAccessor();  
        }


        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            //services.AddDbContext<MyIdentityDbContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            

            // Register repositories, for example:
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IATMRepository, ATMRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
        }
        public static void InitializeDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                dbInitializer.InitializeAsync().Wait();
            }
        }
    }
}

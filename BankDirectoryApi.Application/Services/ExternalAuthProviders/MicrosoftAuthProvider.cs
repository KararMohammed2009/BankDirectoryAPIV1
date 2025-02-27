using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{

    public class MicrosoftAuthProvider : IExternalAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MicrosoftAuthProvider(IConfiguration configuration, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, User? User, AuthenticationDTO? Response)> ValidateAndGetUserAsync(string accessToken)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "HttpContext is null." } } });
                }

                var authProperties = new AuthenticationProperties();
                authProperties.StoreTokens(new[] { new AuthenticationToken { Name = "access_token", Value = accessToken } });


                var authenticateResult = await httpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded)
                {
                    return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Microsoft authentication failed." } } });
                }

                httpContext.User = authenticateResult.Principal;
                var claims = authenticateResult.Principal.Claims;

                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                var phone = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Email not found from Microsoft account." } } });
                }

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        NormalizedUserName = firstName,
                        FamilyName = lastName,
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return (false, null, new AuthenticationDTO { Success = false, Errors = result.Errors });
                    }
                }
            

                return (true, user, null);
        } 
            catch (Exception ex)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = $"Microsoft login error: {ex.Message}" }
    }
});
            }
        }
    }
}

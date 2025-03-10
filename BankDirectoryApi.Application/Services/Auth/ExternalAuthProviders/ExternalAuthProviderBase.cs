using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Auth.ExternalAuthProviders
{
    public abstract class ExternalAuthProviderBase
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly HttpClient _httpClient;
        protected readonly IConfiguration _configration;
        protected readonly IJwtService _jwtService;

        protected ExternalAuthProviderBase(UserManager<IdentityUser> userManager,
            HttpClient httpClient,IConfiguration configuration,IJwtService jwtService)
        {
            _userManager = userManager;
            _httpClient = httpClient;
            _configration = configuration;
            _jwtService = jwtService;
        }

        protected async Task<(bool Success, IEnumerable<IdentityError>? errors, IdentityUser? User, AuthDTO? Response)>
            HandleExternalUserSignIn(
            string id, string? email, string? firstName, string providerName, string accessToken)
        {
            if (string.IsNullOrEmpty(email))
            {
                return (false, new[] { new IdentityError { Description = "Email not found from external provider." } }, null, null);
            }
            if(string.IsNullOrEmpty(firstName))
            {
                return (false, new[] { new IdentityError { Description = "First name not found from external provider." } }, null, null);
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    NormalizedUserName = firstName,
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return (false,result.Errors, null, null);
                }
            }
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id), // 'sub' is Google's unique user ID
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, firstName),
                };

            // Create ClaimsIdentity
            var identity = new ClaimsIdentity(claims, providerName);

            // Create ClaimsPrincipal
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var info = new ExternalLoginInfo(
                              claimsPrincipal,
                              providerName,
                              accessToken,
                              email
                          );

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                return (false,addLoginResult.Errors, null, null);
            }

            //  Generate JWT token for the authenticated user
            var token = await _jwtService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);
            if(token == null || refreshToken == null)
            {
                return (false, new[] { new IdentityError { Description = "Failed to generate JWT tokens." } }, null, null);
            }
            return (true,null, user, new AuthDTO
            {
                Token = token,
                RefreshToken = refreshToken,
            });
        }
    }
}

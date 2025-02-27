using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{
    public class GoogleAuthProvider : IExternalAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public GoogleAuthProvider(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<(bool Success, User? User, AuthenticationDTO? Response)> ValidateAndGetUserAsync(string idToken)
        {
            try
            {
                var clientId = _configuration["Google:ClientId"];
                if (string.IsNullOrEmpty(clientId))
                {
                    throw new InvalidOperationException("Google ClientId is not configured.");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                var email = payload.Email;

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        NormalizedUserName = payload.GivenName,
                        FamilyName = payload.FamilyName,
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return (false, null, new AuthenticationDTO { Success = false, Errors = result.Errors });
                    }
                }

                return (true, user, null);
            }
            catch (InvalidJwtException ex)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = $"Invalid token: {ex.Message}" } } });
            }
            catch (Exception ex)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = $"External login error: {ex.Message}" } } });
            }
        }
    }
}

using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{
    public abstract class ExternalAuthProviderBase
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly HttpClient _httpClient;

        protected ExternalAuthProviderBase(UserManager<IdentityUser> userManager,HttpClient httpClient)
        {
            _userManager = userManager;
            _httpClient = httpClient;
        }

        protected async Task<(bool Success, IdentityUser? User, AuthResponseDTO? Response)> 
            ValidateUserAsync(string email, string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(email))
            {
                return (false, null, new AuthResponseDTO { Success = false, Errors = new[] { new IdentityError { Description = "Email not found from external provider." } } });
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
                    return (false, null, new AuthResponseDTO { Success = false, Errors = result.Errors });
                }
            }

            return (true, user, null);
        }
    }
}

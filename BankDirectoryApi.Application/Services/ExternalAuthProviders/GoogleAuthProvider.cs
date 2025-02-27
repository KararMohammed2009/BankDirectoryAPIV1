using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{
    public class GoogleAuthProvider : ExternalAuthProviderBase, IExternalAuthProvider
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthProvider(UserManager<User> userManager, HttpClient httpClient, IConfiguration configuration)
            : base(userManager, httpClient)
        {
            _configuration = configuration;
        }

        public async Task<(bool Success, User? User, AuthenticationDTO? Response)> ValidateAndGetUserAsync(string idToken)
        {
            var googleApiUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}";
            var response = await _httpClient.GetAsync(googleApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Google token." } } });
            }

            var content = await response.Content.ReadAsStringAsync();
            var googleUser = JsonSerializer.Deserialize<GoogleUserDTO>(content);
            if (googleUser is null)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Google User." } } });
            }
            return await ValidateUserAsync(googleUser.Email, googleUser.GivenName, googleUser.FamilyName);
        }
    }
}

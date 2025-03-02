using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Interfaces.Auth;
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

        public string ProviderName => "Google";
        public GoogleAuthProvider(UserManager<IdentityUser> userManager, HttpClient httpClient)
            : base(userManager, httpClient)
        {

        }
   
        public async Task<(bool Success, IdentityUser? User, AuthResponseDTO? Response)> ValidateAndGetUserAsync(string idToken)
        {
            var googleApiUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}";
            var response = await _httpClient.GetAsync(googleApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null, new AuthResponseDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Google token." } } });
            }

            var content = await response.Content.ReadAsStringAsync();
            var googleUser = JsonSerializer.Deserialize<GoogleUserDTO>(content);
            if (googleUser is null)
            {
                return (false, null, new AuthResponseDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Google User." } } });
            }
            return await ValidateUserAsync(googleUser.Email, googleUser.GivenName, googleUser.FamilyName);
        }
    }
}

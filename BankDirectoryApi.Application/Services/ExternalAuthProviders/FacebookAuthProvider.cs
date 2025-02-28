using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{
    public class FacebookAuthProvider : ExternalAuthProviderBase, IExternalAuthProvider
    {

        private readonly string Name = "Facebook";

        public FacebookAuthProvider(UserManager<User> userManager, HttpClient httpClient)
            : base(userManager, httpClient)
        {

        }
        public string GetProviderName()
        {
            return Name;
        }
        public async Task<(bool Success, User? User, AuthResponseDTO? Response)> ValidateAndGetUserAsync(string accessToken)
        {
           

            var url = $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null, new AuthResponseDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Facebook access token." } } });
            }

            var content = await response.Content.ReadAsStringAsync();
            var facebookUser = JsonSerializer.Deserialize<FacebookUserDTO>(content);
            if(facebookUser is null)
            {
                return (false, null, new AuthResponseDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Facebook User." } } });
            }

            return await ValidateUserAsync(facebookUser.Email, facebookUser.FirstName, facebookUser.LastName);
        }
    }


}

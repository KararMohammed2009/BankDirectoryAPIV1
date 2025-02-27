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
    public class TwitterAuthProvider : ExternalAuthProviderBase, IExternalAuthProvider
    {
        private readonly IConfiguration _configuration;

        public TwitterAuthProvider(UserManager<User> userManager, HttpClient httpClient, IConfiguration configuration)
            : base(userManager, httpClient)
        {
            _configuration = configuration;
        }

        public async Task<(bool Success, User? User, AuthenticationDTO? Response)> ValidateAndGetUserAsync(string accessToken)
        {
          

            var url = $"https://api.twitter.com/2/me?access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Twitter access token." } } });
            }

            var content = await response.Content.ReadAsStringAsync();
            var twitterUser = JsonSerializer.Deserialize<TwitterUserDTO>(content);
            if(twitterUser is null)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Twitter User." } } });
            }

            return await ValidateUserAsync(twitterUser.Email, twitterUser.FirstName, twitterUser.LastName);
        }
    }


}

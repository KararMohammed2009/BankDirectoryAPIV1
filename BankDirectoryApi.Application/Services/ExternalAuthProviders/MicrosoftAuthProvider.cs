using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using BankDirectoryApi.Application.DTOs.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{

    public class MicrosoftAuthProvider : ExternalAuthProviderBase, IExternalAuthProvider
    {
        public MicrosoftAuthProvider(UserManager<User> userManager, HttpClient httpClient)
            : base(userManager, httpClient)
        {
        }

        public async Task<(bool Success, User? User, AuthenticationDTO? Response)> ValidateAndGetUserAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Microsoft access token." } } });
            }

            var content = await response.Content.ReadAsStringAsync();
            var microsoftUser = JsonSerializer.Deserialize<MicrosoftUserDTO>(content);
            if(microsoftUser is null)
            {
                return (false, null, new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid Microsoft User." } } });
            }
            return await ValidateUserAsync(microsoftUser.Mail, microsoftUser.GivenName, microsoftUser.Surname);
        }
    }



}

using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Identity;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankDirectoryApi.Application.Services.ExternalAuthProviders
{
    public class GoogleAuthProvider : ExternalAuthProviderBase, IExternalAuthProvider
    {

        public string ProviderName => "Google";
        public GoogleAuthProvider(
            UserManager<IdentityUser> userManager,
            HttpClient httpClient,IConfiguration configuration,IJwtService jwtService)
            : base(userManager, httpClient,configuration,jwtService)
        {

        }
   
        public async Task<(bool Success, IEnumerable<IdentityError>? errors, IdentityUser? User, AuthResponseDTO? Response)> 
            ManageExternalLogin(string code)
        {
            var accessTokenResult = await GetAccessToken(code);
            if (!accessTokenResult.Success)
            {
                return (false, new[] { new IdentityError { Description = "Failed to get access token." } }, null, null);
            }
            if(accessTokenResult.AccessToken == null)
            {
                return (false, new[] { new IdentityError { Description = "Access token not found." } }, null, null);
            }
            var userResult = await GetUser(accessTokenResult.AccessToken);
            if(!userResult.Success)
            {
                return (false, new[] { new IdentityError { Description = "Failed to get user data." } }, null, null);
            }
            if(userResult.User == null)
            {
                return (false, new[] { new IdentityError { Description = "User data not found." } }, null, null);
            }
           
            return await HandleExternalUserSignIn(
                userResult.User.Sub,userResult.User.Email, userResult.User.Name,
                ProviderName, accessTokenResult.AccessToken);
           

        }



        private async Task<(bool Success,HttpStatusCode StatusCode,string? AccessToken)> 
            GetAccessToken(string code)
        {
            var tokenEndpoint = _configration["Authentication:Google:AccessTokenUrl"];
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", _configration["Authentication:Google:ClientId"]),
            new KeyValuePair<string, string>("client_secret", _configration["Authentication:Google:ClientSecret"]),
            new KeyValuePair<string, string>("redirect_uri", _configration["Authentication:Google:RedirectUri"]),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, response.StatusCode,null);
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenData = System.Text.Json.JsonSerializer.Deserialize<GoogleAccessTokenResponseDTO>(json);

            if (tokenData?.AccessToken == null)
            {
                return (false, response.StatusCode, "Access token not found in response.");
            }

            return (true, response.StatusCode, tokenData.AccessToken);

        }
        private async Task<(bool Success, HttpStatusCode StatusCode, GoogleUserResponseDTO? User)>
            GetUser(string accessToken)
        {
            var tokenEndpoint = _configration["Authentication:Google:UserInfoUrl"];
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync(tokenEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, response.StatusCode, null);
            }
            var UserData = await response.Content.ReadAsStringAsync();
            if(string.IsNullOrEmpty(UserData))
            {
                return (false, response.StatusCode, null);
            }
            return  (true,response.StatusCode, 
                System.Text.Json.JsonSerializer.Deserialize<GoogleUserResponseDTO>(UserData));

        }
    }
}

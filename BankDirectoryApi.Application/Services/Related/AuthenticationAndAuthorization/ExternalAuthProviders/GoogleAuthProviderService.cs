using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    public class GoogleAuthProviderService :  IExternalAuthProviderService
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public GoogleAuthProviderService(
            
            HttpClient httpClient,IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
   
        public async Task<(UserDTO userDTO , string externalAccessToken)> 
            ManageExternalLogin(string code,ClientInfo clientInfo)
        {
            try
            {
                var accessTokenResult = await GetAccessToken(code);
                var userResult = await GetUser(accessTokenResult);
                return  (new UserDTO
                {
                  UserName=userResult.Email,
                  Email=userResult.Email,
                  Id = userResult.Sub
                },accessTokenResult);
            }catch (Exception ex)
            {
                throw new GoogleAuthProviderServiceException("Manage External Login failed", ex);
            }
           

        }



        private async Task<string> 
            GetAccessToken(string code)
        {
            try
            {
                var tokenEndpoint = _configuration["Authentication:Google:AccessTokenUrl"];
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _configuration["Authentication:Google:ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _configuration["Authentication:Google:ClientSecret"]),
                    new KeyValuePair<string, string>("redirect_uri", _configuration["Authentication:Google:RedirectUri"]),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                var response = await _httpClient.PostAsync(tokenEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorContent);
                }

                var json = await response.Content.ReadAsStringAsync();
                if(json == null) throw new Exception("access token response retrieved from external service is null");
                var tokenData = System.Text.Json.JsonSerializer.Deserialize<GoogleAccessTokenResponseDTO>(json);

                if (tokenData == null || tokenData.AccessToken == null)
                {
                    throw new Exception( "Access token not found in response.");
                }

                return tokenData.AccessToken;
            }
            catch (Exception ex) {
                throw new GoogleAuthProviderServiceException("Get AccessToken Failed", ex);
            }

        }
        private async Task< GoogleUserResponseDTO >
            GetUser(string accessToken)
        {
            try
            {
                var tokenEndpoint = _configuration["Authentication:Google:UserInfoUrl"];
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(tokenEndpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorContent);
                }
                var UserData = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(UserData))
                {
                    throw new Exception("user data retrieved from external service is null");
                }
                var userDto = System.Text.Json.JsonSerializer.Deserialize<GoogleUserResponseDTO>(UserData);
                if (userDto == null)
                {
                    throw new Exception("Unable to deserialize user data retrieved from external service");
                }
                return userDto;
            }
            catch (Exception ex) {
            throw new GoogleAuthProviderServiceException($"Get User is Failed", ex);
            }

        }
    }
}

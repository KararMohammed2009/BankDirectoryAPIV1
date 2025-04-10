using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Infrastructure;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    /// <summary>
    /// Service for managing Google external authentication.
    /// </summary>
    public class GoogleAuthProviderService :  IExternalAuthProviderService
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleAuthProviderService> _logger;
        /// <summary>
        /// Constructor for GoogleAuthProviderService.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public GoogleAuthProviderService(
            
            HttpClient httpClient,IConfiguration configuration,
            ILogger<GoogleAuthProviderService> logger
            )
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Manages the external login process for Google authentication.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientInfo"></param>
        /// <returns> A tuple containing the user information and the external access token.</returns>
        public async Task<Result<(UserDTO userDTO , string externalAccessToken)>>
            ManageExternalLogin(string code,ClientInfo clientInfo)
        {
           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(code);
            if (validationResult.IsFailed)
            
                return validationResult.ToResult<(UserDTO userDTO, string externalAccessToken)>();

            var accessTokenResult = await GetAccessToken(code);

            var userResult = await GetUser(accessTokenResult.Value);
            
            return  Result.Ok((new UserDTO
                {
                  UserName=userResult.Value.Email,
                  Email=userResult.Value.Email,
                  Id = userResult.Value.Sub
                },accessTokenResult.Value));
           
           

        }


        /// <summary>
        /// Retrieves the access token from Google using the provided authorization code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns> The access token.</returns>
        private async Task<Result<string> >
            GetAccessToken(string code)
        {
            
                var tokenEndpoint = _configuration["Authentication:Google:AccessTokenUrl"];
                if (string.IsNullOrEmpty(tokenEndpoint))
                {
                    _logger.LogError("Google token endpoint is not configured: _configuration[\"Authentication:Google:AccessTokenUrl\"]");
                    return Result.Fail<string>(new Error("Google token endpoint is not configured")
                        .WithMetadata("ErrorCode",CommonErrors.ConfigurationError));
                }
                var client_id = _configuration["Authentication:Google:ClientId"];
                var client_secret = _configuration["Authentication:Google:ClientSecret"];
                var redirect_uri = _configuration["Authentication:Google:RedirectUri"];
                if (string.IsNullOrEmpty(client_id) || string.IsNullOrEmpty(client_secret) || string.IsNullOrEmpty(redirect_uri))
                {
                    _logger.LogError("Google client_id, client_secret or redirect_uri is not configured : _configuration[\"Authentication:Google:ClientId\"],_configuration[\"Authentication:Google:ClientSecret\"],_configuration[\"Authentication:Google:RedirectUri\"].");
                    return Result.Fail<string>(new Error("Google client_id, client_secret or redirect_uri is not configured")
                        .WithMetadata("ErrorCode", CommonErrors.ConfigurationError));
                }
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", client_id),
                    new KeyValuePair<string, string>("client_secret",client_secret),
                    new KeyValuePair<string, string>("redirect_uri", redirect_uri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                HttpResponseMessage response;
                
                response = await ExternalServiceExceptionHelper.Execute(()=>
                    _httpClient.PostAsync(tokenEndpoint, content),_logger);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Result.Fail<string>(new Error(errorContent)
                        .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
                }

                var json = await response.Content.ReadAsStringAsync();
                if(json == null) 
                    return Result.Fail<string>(new Error("Access token not found in response.")
                        .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
                var tokenData = System.Text.Json.JsonSerializer.Deserialize<GoogleAccessTokenResponseDTO>(json);

                if (tokenData == null || tokenData.AccessToken == null)
                {
                    return Result.Fail<string>(new Error("Access token not found in response.")
                        .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
                }

                return Result.Ok(tokenData.AccessToken);
           

        }
        /// <summary>
        /// Retrieves the user information from Google using the provided access token.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns> The user information.</returns>
        private async Task< Result<GoogleUserResponseDTO >>
            GetUser(string accessToken)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(accessToken);
            if (validationResult.IsFailed)
                return validationResult.ToResult<GoogleUserResponseDTO>();

            var tokenEndpoint = _configuration["Authentication:Google:UserInfoUrl"];
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                _logger.LogError("Google token endpoint is not configured: _configuration[\"Authentication:Google:UserInfoUrl\"]");
                return Result.Fail<GoogleUserResponseDTO>(new Error("Google token endpoint is not configured")
                    .WithMetadata("ErrorCode", CommonErrors.ConfigurationError));
            }

            var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await ExternalServiceExceptionHelper.Execute(() =>
                    client.GetAsync(tokenEndpoint), _logger);
            
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Result.Fail<GoogleUserResponseDTO>(new Error(errorContent)
                        .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
            }
                var UserData = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(UserData))
                return Result.Fail<GoogleUserResponseDTO>(new Error("User data not found in response.")
                    .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
            var userDto = System.Text.Json.JsonSerializer.Deserialize<GoogleUserResponseDTO>(UserData);
                if (userDto == null)
                return Result.Fail<GoogleUserResponseDTO>(new Error("User data not found in response.")
                    .WithMetadata("ErrorCode", CommonErrors.ExternalServiceError));
            return Result.Ok(userDto);
            

        }
    }
}

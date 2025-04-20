

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Represents the response from Google for an access token.
    /// </summary>
    internal class GoogleAccessTokenResponseDTO
    {
        public string? AccessToken { get; set; }
    }
}

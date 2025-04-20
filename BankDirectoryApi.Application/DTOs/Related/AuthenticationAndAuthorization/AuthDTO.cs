
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Represents the authentication data transfer object (DTO) used for authentication and authorization.
    /// </summary>
    public class AuthDTO
    {
        [SwaggerSchema("The access token for authentication.")]
        public string? AccessToken { get; set; }

        [SwaggerSchema("The refresh token to obtain a new access token.")]
        public string? RefreshToken { get; set; }

        [SwaggerSchema("The identifier of the user's session.")]
        public string? SessionId { get; set; }
    }

}

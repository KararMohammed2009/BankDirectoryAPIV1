using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// DTO for logging out a user
    /// </summary>
    public class LogoutUserDTO
    {
        [SwaggerSchema("The identifier of the session to be logged out.", Nullable = false)]
        public required string SessionId { get; set; }
    }
}

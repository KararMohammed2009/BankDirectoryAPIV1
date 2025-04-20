using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Related.Communications
{
    /// <summary>
    /// Data Transfer Object for email confirmation.
    /// </summary>
    public class EmailConfirmationDTO
    {
        [SwaggerSchema("The email address to be confirmed.", Nullable = false)]
        public required string Email { get; set; }

        [SwaggerSchema("The email confirmation token.", Nullable = false)]
        public required string Token { get; set; }
    }
}

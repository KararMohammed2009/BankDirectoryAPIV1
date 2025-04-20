using Swashbuckle.AspNetCore.Annotations;
namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{
    /// <summary>
    /// Data Transfer Object for updating user information.
    /// </summary>
    public class UpdateUserDTO
    {
        [SwaggerSchema("The unique identifier of the user to be updated.", Nullable = false)]
        public required string Id { get; set; }

        [SwaggerSchema("Optional new username for the user. Must be between 3 and 50 characters and can only contain letters, numbers, and underscores if provided.")]
        public string? UserName { get; set; }

        [SwaggerSchema("Optional new email address for the user. Must be in a valid email format if provided.")]
        public string? Email { get; set; }

        [SwaggerSchema("Optional new password for the user. If provided, must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }

        [SwaggerSchema("Optional list of roles for the user.")]
        public IEnumerable<string>? RolesNames { get; set; }

        [SwaggerSchema("Optional new phone number for the user. Must be in a valid international phone number format if provided.")]
        public string? PhoneNumber { get; set; }

        [SwaggerSchema("Optional new value indicating if two-factor authentication is enabled for the user.")]
        public bool TwoFactorEnabled { get; set; }
    }
}

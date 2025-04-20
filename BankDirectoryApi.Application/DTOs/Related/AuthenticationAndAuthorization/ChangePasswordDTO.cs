using Swashbuckle.AspNetCore.Annotations;


namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Represents the data transfer object (DTO) used for changing a user's password.
    /// </summary>
    public class ChangePasswordDTO
    {
        [SwaggerSchema("The email address of the user requesting the password change.", Nullable = false)]
        public required string Email { get; set; }

        [SwaggerSchema("The current password of the user.", Nullable = false)]
        public required string CurrentPassword { get; set; }

        [SwaggerSchema(
            "The new password for the user. Must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character.",
            Nullable = false
        )]
        public required string NewPassword { get; set; }
    }
}

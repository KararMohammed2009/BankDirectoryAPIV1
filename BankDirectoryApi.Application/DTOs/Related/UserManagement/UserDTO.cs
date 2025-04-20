using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{
    /// <summary>
    /// Data Transfer Object for User information.
    /// </summary>
    public class UserDTO
    {
        [SwaggerSchema("The unique identifier of the user.", Nullable = false)]
        public required string Id { get; set; }

        [SwaggerSchema("The username of the user. Must be between 3 and 50 characters and can only contain letters, numbers, and underscores.", Nullable = false)]
        public required string UserName { get; set; }

        [SwaggerSchema("The email address of the user. Must be in a valid email format.", Nullable = false)]
        public required string Email { get; set; }

        [SwaggerSchema("The phone number of the user. Must be in a valid international phone number format if provided.")]
        public string? PhoneNumber { get; set; }

        [SwaggerSchema("The roles assigned to the user.")]
        public IEnumerable<string>? RolesNames { get; set; }

        [SwaggerSchema("Additional claims associated with the user.")]
        public Dictionary<string, string>? Claims { get; set; }
    }
}

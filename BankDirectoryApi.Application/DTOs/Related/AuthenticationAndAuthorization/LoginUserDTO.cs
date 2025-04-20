using Swashbuckle.AspNetCore.Annotations;


namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Data Transfer Object for user login.
    /// </summary>
    public class LoginUserDTO
    {

        [SwaggerSchema("User's email address for login. Must be in a valid email format. If not provided, at least Phone Number or Username must be supplied (one of these fields is required).")]
        public string? Email { get; set; }

        [SwaggerSchema("User's username for login. Must be between 3 and 50 characters and can only contain letters, numbers, and underscores. If not provided, at least Email or Phone Number must be supplied (one of these fields is required).")]
        public string? UserName { get; set; }

        [SwaggerSchema("User's phone number for login. Must be in a valid international phone number format. If not provided, at least Email or Username must be supplied (one of these fields is required).")]
        public string? PhoneNumber { get; set; }

        [SwaggerSchema(
            "User's password for login. It is always required and must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character."
           ,Nullable =false )]
        
        public required string Password { get; set; }
    }


}


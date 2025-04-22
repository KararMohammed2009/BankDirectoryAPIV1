
using Swashbuckle.AspNetCore.Annotations;
namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{
   

        /// <summary>
        /// Data Transfer Object for registering a new user.
        /// </summary>
        public class RegisterUserDTO
        {
            [SwaggerSchema(
                "Optional username for registration. Must be between 3 and 50 characters and can only contain letters, numbers, and underscores if provided."
            )]
            public string? UserName { get; set; }

            [SwaggerSchema(
                "User's email address for registration. Must be in a valid email format if provided. Either Email or Phone Number is required."
            )]
            public string? Email { get; set; }

            [SwaggerSchema(
                "User's password for registration. It is required and must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character.",
                Nullable = false
            )]
            public required string Password { get; set; }


            [SwaggerSchema(
                "User's phone number for registration. Must be in a valid international phone number format if provided. Either Email or Phone Number is required."
            )]
            public string? PhoneNumber { get; set; }

            [SwaggerSchema(
                "Indicates if two-factor authentication is enabled for the user.",
                Nullable = false
            )]
            public required bool TwoFactorEnabled { get; set; }
        }
    
}

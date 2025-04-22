using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Data Transfer Object for resetting a user's password.
    /// </summary>
    public class ResetPasswordDTO
    {

        [SwaggerSchema("The email associated with the user's account.", Nullable = false)]
        public required string Email { get; set; }
        [SwaggerSchema("The password reset code received via email or phone.", Nullable = false)]
        public required string Code { get; set; }

        [SwaggerSchema(
            "The new password for the user. Must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character.",
            Nullable = false
        )]
        public required string NewPassword { get; set; }
    }
}

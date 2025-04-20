using Swashbuckle.AspNetCore.Annotations;


namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    public class ForgotPasswordDTO
    {
        [SwaggerSchema("The email address associated with the account for which the password reset is requested.", Nullable = false)]
        public required string Email { get; set; }
    }
}

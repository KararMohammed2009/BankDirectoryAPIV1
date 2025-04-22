
using Swashbuckle.AspNetCore.Annotations;
namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{


    /// <summary>
    /// Data Transfer Object for registering a new user by an admin.
    /// </summary>
    public class RegisterUserByAdminDTO: RegisterUserDTO
        {
        [SwaggerSchema(
            "Optional list of roles names for the user."
        )]
        public IEnumerable<string>? RolesNames { get; set; }
    }
    
}

using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Data Transfer Object for Role.
    /// </summary>
    public class RoleDTO
    {
        [SwaggerSchema("The unique identifier of the user.", Nullable = false)]
        public required string UserId { get; set; }

        [SwaggerSchema("The name of the role to be assigned or checked.", Nullable = false)]
        public required string RoleName { get; set; }
    }
}

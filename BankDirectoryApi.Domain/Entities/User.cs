using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class User : IdentityUser
    {
        // Add additional properties if needed
        public bool? IsActive { get; set; }
        public string? FamilyName { get; set; }
    }
}

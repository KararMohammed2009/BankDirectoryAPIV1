using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities.Identity
{
    public class ApplicationUserRole : IdentityUserRole<string> 
    {

        public ApplicationRole Role { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

       

    }
}

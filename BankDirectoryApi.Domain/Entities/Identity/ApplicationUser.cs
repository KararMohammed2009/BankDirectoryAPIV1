using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDirectoryApi.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreationDate { get; set; }
        
        public ApplicationUser() {
        }


    }
}

using Microsoft.AspNetCore.Identity;

namespace BankDirectoryApi.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreationDate { get; set; }
    }
}

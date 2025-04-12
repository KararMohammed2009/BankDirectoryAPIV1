using Microsoft.AspNetCore.Identity;

namespace BankDirectoryApi.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreationDate { get; set; }
    }
}

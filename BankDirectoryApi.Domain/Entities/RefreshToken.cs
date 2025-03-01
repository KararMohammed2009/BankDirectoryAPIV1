using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; } // Foreign Key
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }    // When the token was created
        public bool IsRevoked { get; set; }           // A flag to indicate whether the refresh token is revoked
        public DateTime? RevokingDate { get; set; }   // When the token was revoked (if applicable)
        public string IPAddress { get; set; }       // Optional: Store IP address for security
        public string UserAgent { get; set; }       // Optional: Store the user-agent for security


        // Navigation Property
        public IdentityUser User { get; set; } = null!;
    }

}

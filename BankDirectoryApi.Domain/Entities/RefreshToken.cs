using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; } // Primary Key
        [Required]
        public string TokenHash { get; set; } // Hash of the refresh token
        [Required]
        public string UserId { get; set; }  // Foreign Key
        [Required]
        public DateTime ExpirationDate { get; set; } // When the token expires
        [Required]
        public DateTime CreationDate { get; set; }    // When the token was created
        [Required]
        public string CreatedByIp { get; set; } // IP address of the client that created the token
        [Required]
        public bool IsRevoked { get; set; }           // A flag to indicate whether the refresh token is revoked

        public DateTime? RevokingDate { get; set; }   // When the token was revoked (if applicable)

        public string? RevokedByIp { get; set; }      // IP address of the client that revoked the token (if applicable)

        public string UserAgent { get; set; }       // Optional: Store the user-agent for security
        public string? ReplacedByTokenHash { get; set; } // Token that replaced this one (nullable)
        public bool IsUsed { get; set; } // A flag to indicate whether the refresh token is used
        [Required]
        public string SessionId { get; set; } // Session ID for the token
        // Navigation Property
        public IdentityUser User { get; set; } = null!; // The user that owns the refresh token
    }


}

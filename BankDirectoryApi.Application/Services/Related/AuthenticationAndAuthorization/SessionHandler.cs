using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class SessionHandler : ISessionHandler
    {
        public string GenerateNewSessionIdAsync()
        {
            byte[] randomBytes = new byte[64]; // Adjust length as needed
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}

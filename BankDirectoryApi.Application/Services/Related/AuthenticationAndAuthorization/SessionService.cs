using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class SessionService : ISessionService
    {
        public string GenerateNewSessionIdAsync()
        {
            try
            {
                byte[] randomBytes = new byte[64]; // Adjust length as needed
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                var sessionId = Convert.ToBase64String(randomBytes);
                if (string.IsNullOrEmpty(sessionId)) throw new Exception();
                return sessionId;
            }
            catch (Exception e)
            {
                throw new SessionServiceException("Error generating session ID", e);
            }
        }
    }
}

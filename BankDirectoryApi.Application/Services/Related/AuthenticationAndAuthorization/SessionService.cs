using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Services;
using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user sessions
    /// </summary>
    public class SessionService : ISessionService
    {

        private readonly RandomNumberProvider _randomNumberProvider;
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="randomNumberProvider"></param>
        /// <param name="logger"></param>
        public SessionService(RandomNumberProvider randomNumberProvider,ILogger logger)
        {
            _randomNumberProvider = randomNumberProvider;
            _logger = logger;
        }
        /// <summary>
        /// Generate a new session ID
        /// </summary>
        /// <returns>The value of the new session ID</returns>
        public Result<string> GenerateNewSessionIdAsync()
        {

            var sessionId = _randomNumberProvider.GetBase64RandomNumber(64).Value;
            return Result.Ok(sessionId);
            
        }
    }
}

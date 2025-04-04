using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Common.Services;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class SessionService : ISessionService
    {

        private readonly RandomNumberProvider _randomNumberProvider;
        public SessionService(RandomNumberProvider randomNumberProvider)
        {
            _randomNumberProvider = randomNumberProvider;
        }

        public Result<string> GenerateNewSessionIdAsync()
        {

            var sessionId = _randomNumberProvider.GetBase64RandomNumber(64).Value;
            if (string.IsNullOrEmpty(sessionId))
                return Result.Fail(new Error("Generate New SessionId failed")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
                return Result.Ok(sessionId);
            
        }
    }
}

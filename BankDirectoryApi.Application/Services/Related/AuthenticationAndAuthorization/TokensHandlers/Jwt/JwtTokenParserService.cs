using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using System.Security.Cryptography;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using FluentResults;
using System.Net;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenParserService : ITokenParserService
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public JwtTokenParserService(JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }

        public Result<string> GetUserIdAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return Result.Fail(new Error("accessToken is null or empty").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }

            var jwtToken = _jwtSecurityTokenHandler.ReadToken(accessToken) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return Result.Fail(new Error("accessToken is invalid").WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type
            == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null )
            {
                return Result.Fail(new Error("Sub claim not found in accessToken")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            if (string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Result.Fail(new Error("Sub claim in accessToken is empty")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }

            return Result.Ok(userIdClaim.Value);

        }
    }
}
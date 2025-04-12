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
using BankDirectoryApi.Common.Errors;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    /// <summary>
    /// Service to parse JWT tokens
    /// </summary>
    public class JwtTokenParserService : ITokenParserService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JwtTokenParserService()
        {
        }

        /// <summary>
        /// Get User Id from JWT Access Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>The value of the "sub" claim in the JWT token</returns>
        public Result<string> GetUserIdAsync(string accessToken)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(accessToken, "accessToken");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtSecurityTokenHandler.ReadToken(accessToken) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return Result.Fail(new Error("accessToken is invalid")
                    .WithMetadata("ErrorCode", CommonErrors.InvalidToken));
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type
            == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null )
            {
               return Result.Fail(new Error("Sub claim not found in accessToken")
                    .WithMetadata("ErrorCode", CommonErrors.InvalidToken));
            }
            if (string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Result.Fail(new Error("Sub claim in accessToken is empty")
                    .WithMetadata("ErrorCode", CommonErrors.InvalidToken));
            }

            return Result.Ok(userIdClaim.Value);

        }
    }
}
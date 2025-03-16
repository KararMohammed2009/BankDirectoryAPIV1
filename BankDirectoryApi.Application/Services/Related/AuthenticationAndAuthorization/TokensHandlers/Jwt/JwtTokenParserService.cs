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

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenParserService : ITokenParserService
    {

        public JwtTokenParserService()
        {

        }

        public string GetUserIdAsync(string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("Access token null");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    throw new Exception("Access token is invalid");
                }

                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type
                == JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                {
                    throw new Exception("User ID claim not found in token");
                }

                return userIdClaim.Value;
            }
            catch (Exception ex)
            {
                throw new JwtTokenParserServiceException("Error while parsing token", ex);
            }
        }
    }
}
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
using BankDirectoryApi.Common.Exceptions;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using FluentResults;
using System.Net;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.TokensHandlers.Jwt
{
    public class JwtTokenGeneratorService : ITokenGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidProvider _guidProvider;

        public JwtTokenGeneratorService(
            IConfiguration configuration
            ,IDateTimeProvider dateTimeProvider,
            IGuidProvider guidProvider)
        {
            
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
            _guidProvider = guidProvider;
        }

        public Result<string> GenerateAccessToken(string userId
            , string userName, string email, IEnumerable<string>? roles,
            Dictionary<string, string>? userClaims)
        {

            if (userId == null) return Result.Fail(new Error("userId is null")
                .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (userName == null) return
                    Result.Fail(new Error("userName is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            if (email == null)
                return Result.Fail(new Error("email is null")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, _guidProvider.NewGuid().Value.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (userClaims != null)
            {
                foreach (var claim in userClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            var jwtSecret = JwtHelper.GetJwtSecretKey(_configuration).Value;
            var jwtIssuer = JwtHelper.GetJwtIssuer(_configuration).Value;
            var jwtAudience = JwtHelper.GetJwtAudience(_configuration).Value;
            var jwtExpirationHours = JwtHelper.GetJwtExpirationHours(_configuration).Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = _dateTimeProvider.UtcNow.Value.AddHours(jwtExpirationHours);

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            if (string.IsNullOrEmpty(jwtToken))
                return Result.Fail(new Error("Generate JWT Access Token Failed")
                    .WithMetadata("StatusCode", HttpStatusCode.InternalServerError));
            return Result.Ok(jwtToken);
        }


    }
}
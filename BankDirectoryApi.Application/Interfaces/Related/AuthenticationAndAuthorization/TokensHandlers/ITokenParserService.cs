using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    /// <summary>
    /// Service to parse JWT tokens
    /// </summary>
    public interface ITokenParserService
    {
        /// <summary>
        /// Get User Id from JWT Access Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>The value of the "sub" claim in the JWT token</returns>
        Result<string> GetUserIdAsync(string accessToken);
    }
}

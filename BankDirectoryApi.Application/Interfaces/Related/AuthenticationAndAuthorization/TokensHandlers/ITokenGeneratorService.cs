using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    /// <summary>
    /// Service to generate JWT tokens
    /// </summary>
    public interface ITokenGeneratorService
    {
        /// <summary>
        /// Generate JWT Access Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="roles"></param>
        /// <param name="userClaims"></param>
        /// <returns> The value of generated JWT Access Token</returns>
        Result<string> GenerateAccessToken(string userId, string userName, string email, 
            IEnumerable<string>? roles,
            Dictionary<string, string>? userClaims);

    }
}

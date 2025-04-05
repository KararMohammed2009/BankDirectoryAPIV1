using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    public interface ITokenGeneratorService
    {
        Result<string> GenerateAccessToken(string userId, string userName, string email, 
            IEnumerable<string>? roles,
            Dictionary<string, string>? userClaims);

    }
}

using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    public interface ITokenParserService
    {
        Result<string> GetUserIdAsync(string accessToken);
    }
}

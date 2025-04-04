
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface ISessionService
    {
         Result<string> GenerateNewSessionIdAsync();

    }
}

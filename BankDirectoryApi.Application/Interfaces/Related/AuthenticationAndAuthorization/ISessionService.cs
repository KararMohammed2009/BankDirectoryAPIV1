
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service to manage user sessions
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Generate a new session ID
        /// </summary>
        /// <returns>The value of the new session ID</returns>
        Result<string> GenerateNewSessionIdAsync();

    }
}

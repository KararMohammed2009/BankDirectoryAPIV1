
using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public interface IDateTimeProvider
    {
        Result<DateTime> UtcNow { get; }
    }
}

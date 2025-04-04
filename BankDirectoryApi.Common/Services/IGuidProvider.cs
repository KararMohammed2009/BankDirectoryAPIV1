
using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public interface IGuidProvider
    {
        Result<Guid> NewGuid();
    }
}

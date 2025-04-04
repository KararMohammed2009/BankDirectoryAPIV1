
using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public class GuidProvider:IGuidProvider
    {
        public Result<Guid> NewGuid() => Result.Ok(Guid.NewGuid());
    }
}

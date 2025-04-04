
using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public class DateTimeProvider:IDateTimeProvider
    {
        public Result<DateTime> UtcNow => Result.Ok( DateTime.UtcNow);
    }
}

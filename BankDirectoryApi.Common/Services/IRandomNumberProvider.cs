using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public interface IRandomNumberProvider
    {
        Result<string> GetBase64RandomNumber(int length);
    }
}

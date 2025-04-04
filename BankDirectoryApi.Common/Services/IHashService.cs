using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    public interface IHashService
    {
        Result<string> GetHash(string key);
        Result<bool> VerifyHash(string key, string hash);
    }
}

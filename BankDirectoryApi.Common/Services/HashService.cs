
using FluentResults;

namespace BankDirectoryApi.Common.Services
{
     public class HashService : IHashService
    {
        public Result<string> GetHash(string key)
        {
            return Result.Ok(BCrypt.Net.BCrypt.HashString(key));
        }
        public Result<bool> VerifyHash(string key, string hash)
        {
            return Result.Ok(BCrypt.Net.BCrypt.Verify(key, hash));
        }
    }
}

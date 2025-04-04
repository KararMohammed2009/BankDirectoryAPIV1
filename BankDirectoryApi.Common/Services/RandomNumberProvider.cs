
using FluentResults;
using System.Security.Cryptography;

namespace BankDirectoryApi.Common.Services
{
    public class RandomNumberProvider : IRandomNumberProvider
    {
        private readonly RandomNumberGenerator _randomNumberGenerator;
        public RandomNumberProvider(RandomNumberGenerator randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
        }

        public Result<string> GetBase64RandomNumber(int length)
        {
            byte[] randomBytes = new byte[length]; // Adjust length as needed
            _randomNumberGenerator.GetBytes(randomBytes);
          
            return Result.Ok(Convert.ToBase64String(randomBytes));
        }
    }
}

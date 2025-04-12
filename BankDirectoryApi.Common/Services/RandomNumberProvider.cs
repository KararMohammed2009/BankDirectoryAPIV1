
using FluentResults;
using System.Security.Cryptography;

namespace BankDirectoryApi.Common.Services
{
    /// <summary>
    /// Service to generate random numbers
    /// </summary>
    public class RandomNumberProvider : IRandomNumberProvider
    {
        /// <summary>
        /// Random number generator Constructor
        /// </summary>
        public RandomNumberProvider()
        {
        }
        /// <summary>
        /// Generate a random number in Base64 format
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The value of the base64 encoded random number</returns>
        public Result<string> GetBase64RandomNumber(int length)
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[length]; // Adjust length as needed
            randomNumberGenerator.GetBytes(randomBytes);
          
            return Result.Ok(Convert.ToBase64String(randomBytes));
        }
    }
}

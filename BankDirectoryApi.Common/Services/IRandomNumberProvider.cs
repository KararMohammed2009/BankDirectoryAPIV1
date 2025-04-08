using FluentResults;

namespace BankDirectoryApi.Common.Services
{
    /// <summary>
    /// Interface for generating random numbers
    /// </summary>
    public interface IRandomNumberProvider
    {
        /// <summary>
        /// Generate a random number in Base64 format
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The value of the base64 encoded random number</returns>
        Result<string> GetBase64RandomNumber(int length);
    }
}

using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Mappings.Interfaces
{
    /// <summary>
    /// Interface for mapping FluentResults to IActionResult.
    /// </summary>
    public interface IActionGlobalMapper
    {
        /// <summary>
        /// Maps a FluentResult to an IActionResult.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns>The mapped IActionResult.</returns>
        IActionResult MapResultToApiResponse<T>(Result<T> result);
    }
}

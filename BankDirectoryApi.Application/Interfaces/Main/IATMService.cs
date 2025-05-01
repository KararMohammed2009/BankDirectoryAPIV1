using BankDirectoryApi.Application.DTOs.Core.ATMs;
using BankDirectoryApi.Domain.Classes.Pagination;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    /// <summary>
    /// Interface for ATM-related services.
    /// </summary>
    public interface IATMService
    {

        /// <summary>
        /// Retrieves all ATMs asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of ATMs.</returns>
        Task<Result<PaginatedResponse<ATMDTO>>> GetAllATMsAsync(ATMFilterDTO model, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a ATM by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The ATM that matches the ID.</returns>
        Task<Result<ATMDTO>> GetATMByIdAsync(int id);

        Task<Result> AddATMAsync(ATMDTO ATM);
        /// <summary>
        /// Updates an existing ATM asynchronously.
        /// </summary>
        /// <param name="ATM"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> UpdateATMAsync(ATMDTO ATM);
        /// <summary>
        /// Deletes a ATM by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> DeleteATMAsync(int id);
    }
}

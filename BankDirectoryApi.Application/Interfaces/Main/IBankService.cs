using BankDirectoryApi.Application.DTOs.Core.Banks;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    /// <summary>
    /// Interface for bank-related services.
    /// </summary>
    public interface IBankService
    {

        /// <summary>
        /// Retrieves all banks asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of banks.</returns>
        Task<Result<PaginatedResponse<BankDTO>>> GetAllBanksAsync(BankFilterDTO model,CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a bank by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank that matches the ID.</returns>
        Task<Result<BankDTO>> GetBankByIdAsync(int id);
        /// <summary>
        /// Retrieves a bank with its branches by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its branches that matches the ID.</returns>
        Task<Result<BankWithBranchesDTO>> GetBankWithBranchesAsync(int id);
        /// <summary>
        /// Retrieves a bank with its cards by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its cards that matches the ID.</returns>
        Task<Result<BankWithCardsDTO>> GetBankWithCardsAsync(int id);
        /// <summary>
        /// Retrieves a bank with its ATMs by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its ATMs that matches the ID.</returns>
        Task<Result<BankWithATMsDTO>> GetBankWithATMsAsync(int id);
        /// <summary>
        /// Adds a new bank asynchronously.
        /// </summary>
        /// <param name="bank"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> AddBankAsync(BankDTO bank);
        /// <summary>
        /// Updates an existing bank asynchronously.
        /// </summary>
        /// <param name="bank"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> UpdateBankAsync(BankDTO bank);
        /// <summary>
        /// Deletes a bank by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> DeleteBankAsync(int id);
    }
}

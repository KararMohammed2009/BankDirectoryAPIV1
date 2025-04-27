using BankDirectoryApi.Domain.Entities;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for managing banks that extends the generic repository interface.
    /// </summary>
    public interface IBankRepository:IRepository<Bank>
    {
        /// <summary>
        /// Retrieves a bank by its ID with branches
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its branches that matches the ID.</returns>
        Task<Result<Bank>> GetBankWithBranchesAsync(int bankId);
        /// <summary>
        /// Retrieves a bank by its ID with cards
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its cards that matches the ID.</returns>
        Task<Result<Bank>> GetBankWithCardsAsync(int bankId);
        /// <summary>
        /// Retrieves a bank by its ID with ATMs
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its ATMs that matches the ID.</returns>
        Task<Result<Bank>> GetBankWithATMsAsync(int bankId);

    }
}

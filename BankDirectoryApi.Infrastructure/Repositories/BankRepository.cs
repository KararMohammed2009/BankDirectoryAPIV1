using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using BankDirectoryApi.Common.Errors;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository class for managing banks that extends the generic repository class.
    /// </summary>
    public class BankRepository:Repository<Bank>,IBankRepository
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Constructor for BankRepository.
        /// </summary>
        /// <param name="context"></param>
        public BankRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a bank by its ID with branches.
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its branches that matches the ID.</returns>
        public async Task<Result<Bank>> GetBankWithBranchesAsync(int bankId)
        {
            var banksResult = await _context.Banks
                .Include(b => b.Branches)
                .FirstOrDefaultAsync(b => b.Id == bankId);
            if (banksResult == null)
            {
                return Result.Fail(new Error("Error occurred while retrieving the bank with branches.")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }
            return Result.Ok(banksResult);
        }
        /// <summary>
        /// Retrieves a bank by its ID with ATMs.
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its ATMs that matches the ID.</returns>
        public async Task<Result<Bank>> GetBankWithATMsAsync(int bankId)
        {
            var banksResult = await _context.Banks
                .Include(b => b.ATMs)
                .FirstOrDefaultAsync(b => b.Id == bankId);
            if (banksResult == null)
            {
                return Result.Fail(new Error("Error occurred while retrieving the bank with ATMs.")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }
            return Result.Ok(banksResult);
        }
        /// <summary>
        /// Retrieves a bank by its ID with cards.
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>The bank with its cards that matches the ID.</returns>
        public async Task<Result<Bank>> GetBankWithCardsAsync(int bankId)
        {
            var banksResult = await _context.Banks
                .Include(b => b.Cards)
                .FirstOrDefaultAsync(b => b.Id == bankId);
            if (banksResult == null)
            {
                return Result.Fail(new Error("Error occurred while retrieving the bank with Cards.")
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }
            return Result.Ok(banksResult);
        }
    }
}

using BankDirectoryApi.Application.DTOs.Core.Branches;
using BankDirectoryApi.Domain.Classes.Pagination;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    /// <summary>
    /// Interface for Branch-related services.
    /// </summary>
    public interface IBranchService
    {

        /// <summary>
        /// Retrieves all Branches asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of Branches.</returns>
        Task<Result<PaginatedResponse<BranchDTO>>> GetAllBranchesAsync(BranchFilterDTO model, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a Branch by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Branch that matches the ID.</returns>
        Task<Result<BranchDTO>> GetBranchByIdAsync(int id);
       
        Task<Result> AddBranchAsync(BranchDTO Branch);
        /// <summary>
        /// Updates an existing Branch asynchronously.
        /// </summary>
        /// <param name="Branch"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> UpdateBranchAsync(BranchDTO Branch);
        /// <summary>
        /// Deletes a Branch by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> DeleteBranchAsync(int id);
    }
}

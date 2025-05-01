using AutoMapper;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Application.DTOs.Core.Branches;
using Microsoft.Extensions.Logging;
using FluentResults;
using BankDirectoryApi.Domain.Classes.Pagination;

namespace BankDirectoryApi.Application.Services.Main
{
    /// <summary>
    /// Service class for managing Branches.
    /// </summary>
    public class BranchService : IBranchService
    {
        private readonly IMapper _mapper;
        private readonly IBranchRepository _branchRepository;
        private readonly ILogger<BranchService> _logger;
        /// <summary>
        /// Constructor for Brancheservice.
        /// </summary>
        /// <param name="BranchRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public BranchService(IBranchRepository branchRepository, IMapper mapper, ILogger<BranchService> logger)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all Branches asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The paginated list of Branches.</returns>
        public async Task<Result<PaginatedResponse<BranchDTO>>> GetAllBranchesAsync(BranchFilterDTO model,
            CancellationToken cancellationToken)
        {
            var spec = new Specification<Branch>()
            {
                Criteria = ExpressionFilterHelper.CreateFilter<Branch>(model),
                Orderings = OrderingHelper.GetOrderings<Branch>(model.OrderingInfo),
                IsPagingEnabled = model.PaginationInfo != null,
                PageNumber = model.PaginationInfo?.PageNumber,
                PageSize = model.PaginationInfo?.PageSize,
                AsNoTracking = true,
            };

            var BranchesResult = await _branchRepository.GetAllAsync(spec, cancellationToken);
            if (BranchesResult.IsFailed)
                return BranchesResult.ToResult<PaginatedResponse<BranchDTO>>();
            var Branches = BranchesResult.Value;
            var BranchDTOs = _mapper.Map<List<BranchDTO>>(Branches.Items);
            var paginatedResult = BranchesResult.Value.To(BranchDTOs);
            return Result.Ok(paginatedResult);
        }
        /// <summary>
        /// Retrieves a Branch by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Branch that matches the ID.</returns>
        public async Task<Result<BranchDTO>> GetBranchByIdAsync(int id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch.IsFailed) return Result.Fail<BranchDTO>(branch.Errors);
            var BranchDTO = _mapper.Map<BranchDTO>(branch.Value);
            return Result.Ok(BranchDTO);
        }

        /// <summary>
        /// Adds a new Branch asynchronously.
        /// </summary>
        /// <param name="branchDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> AddBranchAsync(BranchDTO branchDTO)
        {
            var branch = _mapper.Map<Branch>(branchDTO);
            await _branchRepository.AddAsync(branch);
            await _branchRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Updates an existing Branch asynchronously.
        /// </summary>
        /// <param name="branchDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> UpdateBranchAsync(BranchDTO branchDTO)
        {
            var branch = _mapper.Map<Branch>(branchDTO);
            _branchRepository.Update(branch);
            await _branchRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Deletes a Branch by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> DeleteBranchAsync(int id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch.IsSuccess)
            {
                _branchRepository.Delete(branch.Value);
                await _branchRepository.SaveChangesAsync();
                return Result.Ok();
            }
            else
            {
                return Result.Fail(branch.Errors);
            }
        }
    }
}

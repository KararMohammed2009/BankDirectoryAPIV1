using AutoMapper;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Application.DTOs.Core.ATMs;
using Microsoft.Extensions.Logging;
using FluentResults;
using BankDirectoryApi.Domain.Classes.Pagination;

namespace BankDirectoryApi.Application.Services.Main
{
    /// <summary>
    /// Service class for managing ATMs.
    /// </summary>
    public class ATMService : IATMService
    {
        private readonly IMapper _mapper;
        private readonly IATMRepository _aTMRepository;
        private readonly ILogger<ATMService> _logger;
        /// <summary>
        /// Constructor for ATMservice.
        /// </summary>
        /// <param name="ATMRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public ATMService(IATMRepository aTMRepository, IMapper mapper, ILogger<ATMService> logger)
        {
            _aTMRepository = aTMRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all ATMs asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The paginated list of ATMs.</returns>
        public async Task<Result<PaginatedResponse<ATMDTO>>> GetAllATMsAsync(ATMFilterDTO model,
            CancellationToken cancellationToken)
        {
            var spec = new Specification<ATM>()
            {
                Criteria = ExpressionFilterHelper.CreateFilter<ATM>(model),
                Orderings = OrderingHelper.GetOrderings<ATM>(model.OrderingInfo),
                IsPagingEnabled = model.PaginationInfo != null,
                PageNumber = model.PaginationInfo?.PageNumber,
                PageSize = model.PaginationInfo?.PageSize,
                AsNoTracking = true,
            };

            var aTMsResult = await _aTMRepository.GetAllAsync(spec, cancellationToken);
            if (aTMsResult.IsFailed)
                return aTMsResult.ToResult<PaginatedResponse<ATMDTO>>();
            var aTMs = aTMsResult.Value;
            var aTMDTOs = _mapper.Map<List<ATMDTO>>(aTMs.Items);
            var paginatedResult = aTMsResult.Value.To(aTMDTOs);
            return Result.Ok(paginatedResult);
        }
        /// <summary>
        /// Retrieves a ATM by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The ATM that matches the ID.</returns>
        public async Task<Result<ATMDTO>> GetATMByIdAsync(int id)
        {
            var aTM = await _aTMRepository.GetByIdAsync(id);
            if (aTM.IsFailed) return Result.Fail<ATMDTO>(aTM.Errors);
            var ATMDTO = _mapper.Map<ATMDTO>(aTM.Value);
            return Result.Ok(ATMDTO);
        }

        /// <summary>
        /// Adds a new ATM asynchronously.
        /// </summary>
        /// <param name="ATMDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> AddATMAsync(ATMDTO ATMDTO)
        {
            var aTM = _mapper.Map<ATM>(ATMDTO);
            await _aTMRepository.AddAsync(aTM);
            await _aTMRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Updates an existing ATM asynchronously.
        /// </summary>
        /// <param name="ATMDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> UpdateATMAsync(ATMDTO ATMDTO)
        {
            var aTM = _mapper.Map<ATM>(ATMDTO);
            _aTMRepository.Update(aTM);
            await _aTMRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Deletes a ATM by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> DeleteATMAsync(int id)
        {
            var aTM = await _aTMRepository.GetByIdAsync(id);
            if (aTM.IsSuccess)
            {
                _aTMRepository.Delete(aTM.Value);
                await _aTMRepository.SaveChangesAsync();
                return Result.Ok();
            }
            else
            {
                return Result.Fail(aTM.Errors);
            }
        }
    }
}

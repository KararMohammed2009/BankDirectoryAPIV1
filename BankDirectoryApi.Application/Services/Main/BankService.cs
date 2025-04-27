using AutoMapper;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Application.DTOs.Core.Banks;
using Microsoft.Extensions.Logging;
using FluentResults;
using BankDirectoryApi.Domain.Classes.Pagination;

namespace BankDirectoryApi.Application.Services.Main
{
    /// <summary>
    /// Service class for managing banks.
    /// </summary>
    public class BankService:IBankService
    {
        private readonly IMapper _mapper;
        private readonly IBankRepository _bankRepository;
        private readonly ILogger<BankService> _logger;
        /// <summary>
        /// Constructor for BankService.
        /// </summary>
        /// <param name="bankRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public BankService(IBankRepository bankRepository,IMapper mapper,ILogger<BankService> logger)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all banks asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The paginated list of banks.</returns>
        public async Task<Result<PaginatedResponse<BankDTO>>> GetAllBanksAsync(BankFilterDTO model,
            CancellationToken cancellationToken)
        {
            var spec = new Specification<Bank>()
            {
                Criteria = ExpressionFilterHelper.CreateFilter<Bank>(model),
                Orderings = OrderingHelper.GetOrderings<Bank>(model.OrderingInfo),
                IsPagingEnabled = model.PaginationInfo != null,
                PageNumber = model.PaginationInfo?.PageNumber,
                PageSize = model.PaginationInfo?.PageSize,
                AsNoTracking = true,
            };
           
            var banksResult = await _bankRepository.GetAllAsync(spec,cancellationToken);
            if (banksResult.IsFailed)
                return banksResult.ToResult<PaginatedResponse<BankDTO>>();
            var banks = banksResult.Value;
            var bankDTOs = _mapper.Map<List<BankDTO>>(banks);
            var paginatedResult = banksResult.Value.To(bankDTOs);
            return Result.Ok(paginatedResult);
        }
        /// <summary>
        /// Retrieves a bank by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank that matches the ID.</returns>
        public async Task<Result<BankDTO>> GetBankByIdAsync(int id)
        {
            var bank =  await _bankRepository.GetByIdAsync(id);
            if (bank.IsFailed) return Result.Fail<BankDTO>(bank.Errors);
            var bankDTO = _mapper.Map<BankDTO>(bank.Value);
            return Result.Ok(bankDTO);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its branches that matches the ID.</returns>
        public async Task<Result<BankWithBranchesDTO>> GetBankWithBranchesAsync(int id)
        {
            var banksWithBranches = await _bankRepository.GetBankWithBranchesAsync(id);
            if (banksWithBranches.IsFailed) return Result.Fail<BankWithBranchesDTO>(banksWithBranches.Errors);
            return Result.Ok(_mapper.Map<BankWithBranchesDTO>(banksWithBranches));
        }
        /// <summary>
        /// Retrieves a bank with its cards by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its cards that matches the ID.</returns>
        public async Task<Result<BankWithCardsDTO>> GetBankWithCardsAsync(int id)
        {
            var banksWithCards = await _bankRepository.GetBankWithCardsAsync(id);
            if (banksWithCards.IsFailed) return Result.Fail<BankWithCardsDTO>(banksWithCards.Errors);
            return Result.Ok(_mapper.Map<BankWithCardsDTO>(banksWithCards));
        }
        /// <summary>
        /// Retrieves a bank with its ATMs by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bank with its ATMs that matches the ID.</returns>
        public async Task<Result<BankWithATMsDTO>> GetBankWithATMsAsync(int id)
        {
            var banksWithATMs = await _bankRepository.GetBankWithATMsAsync(id);
            if (banksWithATMs.IsFailed) return Result.Fail<BankWithATMsDTO>(banksWithATMs.Errors);
            return Result.Ok(_mapper.Map<BankWithATMsDTO>(banksWithATMs));
        }
        /// <summary>
        /// Adds a new bank asynchronously.
        /// </summary>
        /// <param name="bankdto"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> AddBankAsync(BankDTO bankdto)
        {
            var bank = _mapper.Map<Bank>(bankdto);
            await _bankRepository.AddAsync(bank);
            await _bankRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Updates an existing bank asynchronously.
        /// </summary>
        /// <param name="bankdto"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> UpdateBankAsync(BankDTO bankdto)
        {
            var bank = _mapper.Map<Bank>(bankdto);
            _bankRepository.Update(bank);
            await _bankRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Deletes a bank by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> DeleteBankAsync(int id)
        {
            var bank = await _bankRepository.GetByIdAsync(id);
            if (bank.IsSuccess)
            {
                _bankRepository.Delete(bank.Value);
                await _bankRepository.SaveChangesAsync();
                return Result.Ok();
            }
            else
            {
                return Result.Fail(bank.Errors);
            }
        }
    }
}

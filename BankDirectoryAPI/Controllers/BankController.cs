using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Core.Banks;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Pagination;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers
{
    /// <summary>
    /// Controller for managing bank-related operations.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Banks")]
    [ApiController]
    public class BankController : Controller
    {
        private readonly IBankService _bankService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        public BankController(IBankService bankService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _bankService = bankService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>
        /// Retrieves all banks.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBanks([FromQuery] BankFilterDTO model, CancellationToken cancellationToken)
        {
            var result = await _bankService.GetAllBanksAsync(model, cancellationToken);
            return _actionGlobalMapper.MapResultToApiResponse<Result<PaginatedResponse<BankDTO>>>(result);
        }
        /// <summary>
        /// Retrieves a bank by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            var bank = await _bankService.GetBankByIdAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankDTO>>(bank);
        }
        /// <summary>
        /// Retrieves a bank along with its branches by bank ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet("{id}/Branches")]
        public async Task<IActionResult> GetBankWithBranches(int id)
        {
            var bank = await _bankService.GetBankWithBranchesAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankWithBranchesDTO>>(bank);
        }
        /// <summary>
        /// Retrieves a bank along with its cards by bank ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet("{id}/Cards")]
        public async Task<IActionResult> GetBankWithCards(int id)
        {
            var bank = await _bankService.GetBankWithCardsAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankWithCardsDTO>>(bank);
        }
        /// <summary>
        /// Retrieves a bank along with its ATMs by bank ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet("{id}/ATMs")]
        public async Task<IActionResult> GetBankWithATMs(int id)
        {
            var bank = await _bankService.GetBankWithATMsAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankWithATMsDTO>>(bank);
        }
        /// <summary>
        /// Adds a new bank to the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the addition process.</returns>
        [HttpPost]
        public async Task<IActionResult> AddBank([FromBody] BankDTO model)
        {
            var result = await _bankService.AddBankAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankDTO>>(result);
        }
        /// <summary>
        /// Updates an existing bank's information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankdto"></param>
        /// <returns>the result of the update process.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateBank([FromBody] BankUpdateDTO model)
        {
            var result = await _bankService.UpdateBankAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankUpdateDTO>>(result);
        }
        /// <summary>
        /// Deletes a bank from the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the deletion process.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var result = await _bankService.DeleteBankAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BankUpdateDTO>>(result);
        }
    }
}

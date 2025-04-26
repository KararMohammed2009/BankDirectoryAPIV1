using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Core.Bank;
using BankDirectoryApi.Application.Interfaces.Main;
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
        public async Task<IActionResult> GetAllBanks(BankFilterDTO model,CancellationToken cancellationToken)
        {
          
           var result = await _bankService.GetAllBanksAsync(model, cancellationToken);
            return _actionGlobalMapper.MapResultToApiResponse(result);
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
            return _actionGlobalMapper.MapResultToApiResponse(bank);
        }
        /// <summary>
        /// Retrieves a bank along with its branches by bank ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the retrieval process.</returns>
        [HttpGet("{id}/branches")]
        public async Task<IActionResult> GetBankWithBranches(int id)
        {
            var bank = await _bankService.GetBankWithBranchesAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse(bank);
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
            return _actionGlobalMapper.MapResultToApiResponse(result);
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
            return _actionGlobalMapper.MapResultToApiResponse(result);
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
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
    }
}

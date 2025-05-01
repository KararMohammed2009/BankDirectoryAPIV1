using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Core.ATMs;
using BankDirectoryApi.Application.Interfaces.Main;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ATMDirectoryApi.API.Controllers
{
    /// <summary>  
    /// Controller for managing ATM-related operations.  
    /// </summary>  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ATMs")]
    [ApiController]
    public class ATMController : Controller
    {
        private readonly IATMService _ATMService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        public ATMController(IATMService ATMService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _ATMService = ATMService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>  
        /// Retrieves all ATMs.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <param name="cancellationToken"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet]
        public async Task<IActionResult> GetAllATMs([FromQuery] ATMFilterDTO model, CancellationToken cancellationToken)
        {
            var result = await _ATMService.GetAllATMsAsync(model, cancellationToken);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
        /// <summary>  
        /// Retrieves a ATM by its ID.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetATMById(int id)
        {
            var ATM = await _ATMService.GetATMByIdAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse(ATM);
        }

        /// <summary>  
        /// Adds a new ATM to the system.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <returns>The result of the addition process.</returns>  
        [HttpPost]
        public async Task<IActionResult> AddATM([FromBody] ATMDTO model)
        {
            var result = await _ATMService.AddATMAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<bool>>(result);
        }
        /// <summary>  
        /// Updates an existing ATM's information.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <param name="ATMdto"></param>  
        /// <returns>the result of the update process.</returns>  
        [HttpPut]
        public async Task<IActionResult> UpdateATM([FromBody] ATMUpdateDTO model)
        {
            var result = await _ATMService.UpdateATMAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<ATMUpdateDTO>>(result);
        }
        /// <summary>  
        /// Deletes a ATM from the system.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the deletion process.</returns>  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteATM(int id)
        {
            var result = await _ATMService.DeleteATMAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<ATMUpdateDTO>>(result);
        }
    }
}

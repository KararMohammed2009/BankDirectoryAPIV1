using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Core.Branches;
using BankDirectoryApi.Application.Interfaces.Main;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BranchDirectoryApi.API.Controllers
{
    /// <summary>  
    /// Controller for managing Branch-related operations.  
    /// </summary>  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Branches")]
    [ApiController]
    public class BranchController : Controller
    {
        private readonly IBranchService _BranchService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        public BranchController(IBranchService BranchService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _BranchService = BranchService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>  
        /// Retrieves all Branches.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <param name="cancellationToken"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet]
        public async Task<IActionResult> GetAllBranchs([FromQuery] BranchFilterDTO model, CancellationToken cancellationToken)
        {
            var result = await _BranchService.GetAllBranchesAsync(model, cancellationToken);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
        /// <summary>  
        /// Retrieves a Branch by its ID.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var Branch = await _BranchService.GetBranchByIdAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse(Branch);
        }
       
        /// <summary>  
        /// Adds a new Branch to the system.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <returns>The result of the addition process.</returns>  
        [HttpPost]
        public async Task<IActionResult> AddBranch([FromBody] BranchDTO model)
        {
            var result = await _BranchService.AddBranchAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<bool>>(result);
        }
        /// <summary>  
        /// Updates an existing Branch's information.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <param name="Branchdto"></param>  
        /// <returns>the result of the update process.</returns>  
        [HttpPut]
        public async Task<IActionResult> UpdateBranch([FromBody] BranchUpdateDTO model)
        {
            var result = await _BranchService.UpdateBranchAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BranchUpdateDTO>>(result);
        }
        /// <summary>  
        /// Deletes a Branch from the system.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the deletion process.</returns>  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _BranchService.DeleteBranchAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<BranchUpdateDTO>>(result);
        }
    }
}

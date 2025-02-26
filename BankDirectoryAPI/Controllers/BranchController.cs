using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Services;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService BranchService)
        {
            _branchService = BranchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var branchs = await _branchService.GetAllBranchesAsync();
            return Ok(branchs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null) return NotFound();
            return Ok(branch);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddBranch([FromBody] Branch branch)
        {
            await _branchService.AddBranchAsync(branch);
            return CreatedAtAction(nameof(GetBranchById), new { id = branch.Id }, branch);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] Branch branch)
        {
            if (id != branch.Id) return BadRequest();
            await _branchService.UpdateBranchAsync(branch);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            await _branchService.DeleteBranchAsync(id);
            return NoContent();
        }
    }
}

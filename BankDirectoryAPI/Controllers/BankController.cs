using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : Controller
    {
        private readonly IBankService _bankService;
        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBanks()
        {
          
            var banks = await _bankService.GetAllBanksAsync();
            return Ok(banks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            var bank = await _bankService.GetBankByIdAsync(id);
            if (bank == null) return NotFound();
            return Ok(bank);
        }
        [HttpGet("{id}/branches")]
        public async Task<IActionResult> GetBankWithBranches(int id)
        {
            var bank = await _bankService.GetBankWithBranchesAsync(id);
            if (bank == null) return NotFound();
            return Ok(bank);
        }
        [HttpPost]
        public async Task<IActionResult> AddBank([FromBody] BankDTO bankdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _bankService.AddBankAsync(bankdto);
            return CreatedAtAction(nameof(GetBankById), new { id = bankdto.Id }, bankdto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankDTO bankdto)
        {
            if (id != bankdto.Id) return BadRequest();
            await _bankService.UpdateBankAsync(bankdto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            await _bankService.DeleteBankAsync(id);
            return NoContent();
        }
    }
}

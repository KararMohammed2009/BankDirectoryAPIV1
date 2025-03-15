using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Application.Services;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ATMController : Controller
    {
        private readonly IATMService _aTMService;
        public ATMController(IATMService ATMService)
        {
            _aTMService = ATMService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllATMs()
        {
            var aTMs = await _aTMService.GetAllATMsAsync();
            return Ok(aTMs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetATMById(int id)
        {
            var aTM = await _aTMService.GetATMByIdAsync(id);
            if (aTM == null) return NotFound();
            return Ok(aTM);
        }

        [HttpPost]
        public async Task<IActionResult> AddATM([FromBody] ATM aTM)
        {
            await _aTMService.AddATMAsync(aTM);
            return CreatedAtAction(nameof(GetATMById), new { id = aTM.Id }, aTM);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateATM(int id, [FromBody] ATM aTM)
        {
            if (id != aTM.Id) return BadRequest();
            await _aTMService.UpdateATMAsync(aTM);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteATM(int id)
        {
            await _aTMService.DeleteATMAsync(id);
            return NoContent();
        }
    }
}


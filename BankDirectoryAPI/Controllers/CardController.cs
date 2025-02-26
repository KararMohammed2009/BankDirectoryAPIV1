using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Services;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        public CardController(ICardService CardService)
        {
            _cardService = CardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCardes()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            await _cardService.AddCardAsync(card);
            return CreatedAtAction(nameof(GetCardById), new { id = card.Id }, card);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] Card card)
        {
            if (id != card.Id) return BadRequest();
            await _cardService.UpdateCardAsync(card);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            await _cardService.DeleteCardAsync(id);
            return NoContent();
        }
    }
}


using Asp.Versioning;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Core.Cards;
using BankDirectoryApi.Application.Interfaces.Main;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace CardDirectoryApi.API.Controllers
{
    /// <summary>  
    /// Controller for managing Card-related operations.  
    /// </summary>  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Cards")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly ICardService _CardService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        public CardController(ICardService CardService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _CardService = CardService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>  
        /// Retrieves all Cards.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <param name="cancellationToken"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet]
        public async Task<IActionResult> GetAllCards([FromQuery] CardFilterDTO model, CancellationToken cancellationToken)
        {
            var result = await _CardService.GetAllCardsAsync(model, cancellationToken);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
        /// <summary>  
        /// Retrieves a Card by its ID.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the retrieval process.</returns>  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var Card = await _CardService.GetCardByIdAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse(Card);
        }

        /// <summary>  
        /// Adds a new Card to the system.  
        /// </summary>  
        /// <param name="model"></param>  
        /// <returns>The result of the addition process.</returns>  
        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] CardDTO model)
        {
            var result = await _CardService.AddCardAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<bool>>(result);
        }
        /// <summary>  
        /// Updates an existing Card's information.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <param name="Carddto"></param>  
        /// <returns>the result of the update process.</returns>  
        [HttpPut]
        public async Task<IActionResult> UpdateCard([FromBody] CardUpdateDTO model)
        {
            var result = await _CardService.UpdateCardAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse<Result<CardUpdateDTO>>(result);
        }
        /// <summary>  
        /// Deletes a Card from the system.  
        /// </summary>  
        /// <param name="id"></param>  
        /// <returns>The result of the deletion process.</returns>  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var result = await _CardService.DeleteCardAsync(id);
            return _actionGlobalMapper.MapResultToApiResponse<Result<CardUpdateDTO>>(result);
        }
    }
}

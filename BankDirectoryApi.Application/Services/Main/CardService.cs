using AutoMapper;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Application.DTOs.Core.Cards;
using Microsoft.Extensions.Logging;
using FluentResults;
using BankDirectoryApi.Domain.Classes.Pagination;

namespace BankDirectoryApi.Application.Services.Main
{
    /// <summary>
    /// Service class for managing Cards.
    /// </summary>
    public class CardService : ICardService
    {
        private readonly IMapper _mapper;
        private readonly ICardRepository _cardRepository;
        private readonly ILogger<CardService> _logger;
        /// <summary>
        /// Constructor for Cardservice.
        /// </summary>
        /// <param name="CardRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public CardService(ICardRepository cardRepository, IMapper mapper, ILogger<CardService> logger)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all Cards asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The paginated list of Cards.</returns>
        public async Task<Result<PaginatedResponse<CardDTO>>> GetAllCardsAsync(CardFilterDTO model,
            CancellationToken cancellationToken)
        {
            var spec = new Specification<Card>()
            {
                Criteria = ExpressionFilterHelper.CreateFilter<Card>(model),
                Orderings = OrderingHelper.GetOrderings<Card>(model.OrderingInfo),
                IsPagingEnabled = model.PaginationInfo != null,
                PageNumber = model.PaginationInfo?.PageNumber,
                PageSize = model.PaginationInfo?.PageSize,
                AsNoTracking = true,
            };

            var CardsResult = await _cardRepository.GetAllAsync(spec, cancellationToken);
            if (CardsResult.IsFailed)
                return CardsResult.ToResult<PaginatedResponse<CardDTO>>();
            var Cards = CardsResult.Value;
            var CardDTOs = _mapper.Map<List<CardDTO>>(Cards.Items);
            var paginatedResult = CardsResult.Value.To(CardDTOs);
            return Result.Ok(paginatedResult);
        }
        /// <summary>
        /// Retrieves a Card by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Card that matches the ID.</returns>
        public async Task<Result<CardDTO>> GetCardByIdAsync(int id)
        {
            var Card = await _cardRepository.GetByIdAsync(id);
            if (Card.IsFailed) return Result.Fail<CardDTO>(Card.Errors);
            var CardDTO = _mapper.Map<CardDTO>(Card.Value);
            return Result.Ok(CardDTO);
        }
       
        /// <summary>
        /// Adds a new Card asynchronously.
        /// </summary>
        /// <param name="cardDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> AddCardAsync(CardDTO cardDTO)
        {
            var card = _mapper.Map<Card>(cardDTO);
            await _cardRepository.AddAsync(card);
            await _cardRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Updates an existing Card asynchronously.
        /// </summary>
        /// <param name="cardDTO"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> UpdateCardAsync(CardDTO cardDTO)
        {
            var card = _mapper.Map<Card>(cardDTO);
            _cardRepository.Update(card);
            await _cardRepository.SaveChangesAsync();
            return Result.Ok();
        }
        /// <summary>
        /// Deletes a Card by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation weather it was successful or not.</returns>
        public async Task<Result> DeleteCardAsync(int id)
        {
            var card = await _cardRepository.GetByIdAsync(id);
            if (card.IsSuccess)
            {
                _cardRepository.Delete(card.Value);
                await _cardRepository.SaveChangesAsync();
                return Result.Ok();
            }
            else
            {
                return Result.Fail(card.Errors);
            }
        }
    }
}

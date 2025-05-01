using BankDirectoryApi.Application.DTOs.Core.Cards;
using BankDirectoryApi.Domain.Classes.Pagination;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    /// <summary>
    /// Interface for Card-related services.
    /// </summary>
    public interface ICardService
    {

        /// <summary>
        /// Retrieves all Cards asynchronously with filtering options.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of Cards.</returns>
        Task<Result<PaginatedResponse<CardDTO>>> GetAllCardsAsync(CardFilterDTO model, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a Card by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Card that matches the ID.</returns>
        Task<Result<CardDTO>> GetCardByIdAsync(int id);

        Task<Result> AddCardAsync(CardDTO Card);
        /// <summary>
        /// Updates an existing Card asynchronously.
        /// </summary>
        /// <param name="Card"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> UpdateCardAsync(CardDTO Card);
        /// <summary>
        /// Deletes a Card by its ID asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The result of the operation.</returns>
        Task<Result> DeleteCardAsync(int id);
    }
}

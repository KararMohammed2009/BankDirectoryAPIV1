using BankDirectoryApi.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Cards
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a card.
    /// </summary>
    public class CardDTO
    {
        [SwaggerSchema("The unique identifier of the card.", Nullable = true)]
        public int Id { get; set; }

        [SwaggerSchema("The ID of the bank that issues this card (Foreign Key).", Nullable = false)]
        public required int BankId { get; set; } // Foreign Key

        [SwaggerSchema("The name of the card (e.g., 'Gold Rewards', 'Basic Checking').", Nullable = false)]
        public required string Name { get; set; }

        [SwaggerSchema("The type of the card (e.g., 'Credit', 'Debit', 'Prepaid').", Nullable = false)]
        public required CardType Type { get; set; }

        [SwaggerSchema("The annual fee associated with the card, if any.")]
        public decimal? AnnualFee { get; set; }

        [SwaggerSchema("A brief description of the card and its benefits.")]
        public string? Description { get; set; }
    }
}

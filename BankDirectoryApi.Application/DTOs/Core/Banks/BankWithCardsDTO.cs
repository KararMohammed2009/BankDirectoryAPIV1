using BankDirectoryApi.Application.DTOs.Core.Cards;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Banks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank with associated cards.
    /// </summary>
    public class BankWithCardsDTO : BankDTO
    {
        [SwaggerSchema("A list of Cards associated with this bank.")]
        public List<CardDTO>? Cards { get; set; }
    }
}

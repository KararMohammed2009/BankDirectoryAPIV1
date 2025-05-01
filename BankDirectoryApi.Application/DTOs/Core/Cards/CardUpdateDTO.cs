using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Cards
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a card update.
    /// </summary>
    public class CardUpdateDTO :CardDTO
    {
        [SwaggerSchema("The unique identifier of the card.", Nullable = false)]
        public new required int Id { get; set; }
    }
}

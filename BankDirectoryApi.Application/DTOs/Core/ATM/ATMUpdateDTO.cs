using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.ATM
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an ATM update.
    /// </summary>
    public class ATMUpdateDTO :ATMDTO
    {
        [SwaggerSchema("The unique identifier of the ATM.", Nullable = false)]
        public new required int Id { get; set; }

    }
}

using BankDirectoryApi.Domain.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.ATMs
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an ATM.
    /// </summary>
    public class ATMDTO
    {
       
            [SwaggerSchema("The unique identifier of the ATM.", Nullable = true)]
            public int Id { get; set; }

            [SwaggerSchema("The ID of the bank that owns this ATM (Foreign Key).", Nullable = false)]
            public required int BankId { get; set; } // Foreign Key

            [SwaggerSchema("The geographical coordinates of the ATM's location.", Nullable = false)]
            public required GeoCoordinate? GeoCoordinate { get; set; }

            [SwaggerSchema("Indicates whether the ATM is currently operational.", Nullable = false)]
            public required bool IsOperational { get; set; }
        
    }
}

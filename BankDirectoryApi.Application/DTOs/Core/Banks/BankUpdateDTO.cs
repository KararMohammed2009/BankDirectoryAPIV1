using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Banks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank update.
    /// </summary>
    public class BankUpdateDTO:BankDTO
    {
        [SwaggerSchema("The unique identifier of the bank.", Nullable = false)]
        public new required int Id { get; set; }

       
    }
}

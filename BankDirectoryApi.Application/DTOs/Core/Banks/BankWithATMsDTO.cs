
using BankDirectoryApi.Application.DTOs.Core.ATMs;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Banks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank with associated ATMs.
    /// </summary>
    public class BankWithATMsDTO : BankDTO
    {
        [SwaggerSchema("A list of ATMs associated with this bank.")]
        public List<ATMDTO>? ATMs { get; set; }

    }
}

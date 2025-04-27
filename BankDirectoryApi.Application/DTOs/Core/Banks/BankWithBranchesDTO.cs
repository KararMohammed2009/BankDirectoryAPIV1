using BankDirectoryApi.Application.DTOs.Core.Branches;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Banks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank with its associated branches.
    /// </summary>
    public class BankWithBranchesDTO : BankDTO
    {
        [SwaggerSchema("A list of branches associated with this bank.")]
        public List<BranchDTO>? Branches { get; set; }

    }
}

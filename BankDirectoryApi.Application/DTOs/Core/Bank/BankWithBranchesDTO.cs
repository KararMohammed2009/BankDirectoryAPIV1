using BankDirectoryApi.Application.DTOs.Core.Branch;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Bank
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

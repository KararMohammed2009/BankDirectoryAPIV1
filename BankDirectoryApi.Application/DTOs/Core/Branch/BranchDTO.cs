using BankDirectoryApi.Domain.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Branch
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank branch.
    /// </summary>
    public class BranchDTO 
    {
        [SwaggerSchema("The unique identifier of the branch.", Nullable = true)]
        public int Id { get; set; }

        [SwaggerSchema("The ID of the bank this branch belongs to (Foreign Key).", Nullable = false)]
        public required int BankId { get; set; } // Foreign Key

        [SwaggerSchema("The name of the bank branch.", Nullable = false)]
        public required string Name { get; set; }

        [SwaggerSchema("The physical address of the bank branch.")]
        public Address? Address { get; set; }

        [SwaggerSchema("The geographical coordinates of the bank branch.")]
        public GeoCoordinate? GeoCoordinate { get; set; }

        [SwaggerSchema("The customer support phone number for this specific branch.")]
        public string? CustomerSupportNumber { get; set; }

    }
}

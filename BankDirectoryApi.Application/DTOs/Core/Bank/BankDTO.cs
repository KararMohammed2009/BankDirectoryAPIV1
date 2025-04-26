using BankDirectoryApi.Domain.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Core.Bank
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bank.
    /// </summary>
    public class BankDTO
    {
        [SwaggerSchema("The unique identifier of the bank.", Nullable = true)]
        public int Id { get; set; }

        [SwaggerSchema("The name of the bank.", Nullable = false)]
        public required string Name { get; set; } = string.Empty;

        [SwaggerSchema("A unique code assigned to the bank.", Nullable = false)]
        public required string Code { get; set; }

        [SwaggerSchema("The official website URL of the bank.")]
        public string? Website { get; set; }

        [SwaggerSchema("The customer support phone number of the bank.")]
        public string? CustomerSupportNumber { get; set; }

        [SwaggerSchema("The geographical coordinates of the bank.")]
        public GeoCoordinate? GeoCoordinate { get; set; }

        [SwaggerSchema("The physical address of the bank.")]
        public Address? Address { get; set; }
    }
}

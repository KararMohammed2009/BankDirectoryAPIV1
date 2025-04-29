using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
namespace BankDirectoryApi.Application.DTOs.Core.Banks
{
    /// <summary>
    /// Data Transfer Object for filtering banks.
    /// </summary>
    public class BankFilterDTO
    {
        [SwaggerSchema("The unique identifier of the bank.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Bank), nameof(Bank.Name))]
        public string? BankName { get; set; }

        [SwaggerSchema("The customer support phone number of the bank.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Bank), nameof(Bank.CustomerSupportNumber))]
        public string? CustomerSupportNumber { get; set; }

        [SwaggerSchema("The code of the bank.", Nullable = true)]
        [Filter(FilterType.StartsWith, typeof(Bank), nameof(Bank.Code))]
        public string? BankCode { get; set; }

        [SwaggerSchema("the pagination information.", Nullable = true)]
        public PaginationInfo? PaginationInfo { get; set; }
        [SwaggerSchema("The ordering information.", Nullable = true)]
        public Dictionary<string,string>? OrderingInfo { get; set; }
    }
}

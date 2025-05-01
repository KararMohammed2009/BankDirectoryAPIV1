using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Core.ATMs
{
    /// <summary>
    /// DTO for filtering ATMs.
    /// </summary>
    public class ATMFilterDTO
    {
        [SwaggerSchema("The bank ID of the ATM.", Nullable = true)]
        [Filter(FilterType.Equals, typeof(ATM), nameof(ATM.BankId))]
        public int? BankId { get; set; }
        [SwaggerSchema("The geographical coordinates of the ATM.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(ATM), nameof(ATM.GeoCoordinate))]
        public string? GeoCoordinate { get; set; }
        [SwaggerSchema("The operational status of the ATM.", Nullable = true)]
        [Filter(FilterType.Equals, typeof(ATM), nameof(ATM.IsOperational))]
        public bool? IsOperational { get; set; }
        [SwaggerSchema("The pagination information.", Nullable = true)]
        public PaginationInfo? PaginationInfo { get; set; }
        [SwaggerSchema("The ordering information.", Nullable = true)]
        public Dictionary<string, string>? OrderingInfo { get; set; }
    }
}

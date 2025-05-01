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

namespace BankDirectoryApi.Application.DTOs.Core.Branches
{
    /// <summary>
    /// DTO for filtering branches.
    /// </summary>
    public class BranchFilterDTO
    {
        [SwaggerSchema("The name of the branch.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Branch), nameof(Branch.Name))]
        public string? BranchName { get; set; }
        [SwaggerSchema("The address of the branch.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Branch), nameof(Branch.Address))]
        public string? Address { get; set; }
        [SwaggerSchema("The customer support number of the branch.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Branch), nameof(Branch.CustomerSupportNumber))]
        public string? CustomerSupportNumber { get; set; }
        [SwaggerSchema("The geographical coordinates of the branch.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Branch), nameof(Branch.GeoCoordinate))]
        public string? GeoCoordinate { get; set; }
        [SwaggerSchema("The bank ID of the branch.", Nullable = true)]
        [Filter(FilterType.Equals, typeof(Branch), nameof(Branch.BankId))]
        public int? BankId { get; set; }
        [SwaggerSchema("the pagination information.", Nullable = true)]
        public PaginationInfo? PaginationInfo { get; set; }
        [SwaggerSchema("The ordering information.", Nullable = true)]
        public Dictionary<string, string>? OrderingInfo { get; set; }
    }
}

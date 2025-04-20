using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Enums;
using BankDirectoryApi.Domain.Entities.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{
    public class UserFilterDTO
    {
        [Filter(FilterType.Contains, typeof(ApplicationUser), nameof(ApplicationUser.UserName))]
        [SwaggerSchema("UserName", Description = "The username of the user. This is a partial match filter.")]
        public string? UserName { get; set; }

        [Filter(FilterType.Contains, typeof(ApplicationUser), nameof(ApplicationUser.Email))]
        [SwaggerSchema("EmailAddress", Description = "The email address of the user. This is a partial match filter.")]
        public string? EmailAddress { get; set; }

        [Filter(FilterType.StartsWith, typeof(ApplicationUser), nameof(ApplicationUser.PhoneNumber))]
        [SwaggerSchema("PhoneNumber", Description = "The phone number of the user. This is a partial match filter.")]
        public string? PhoneNumber { get; set; }

        [Filter(FilterType.StartsWith, typeof(ApplicationRole), nameof(ApplicationRole.Name))]
        [SwaggerSchema("RoleName", Description = "The name of the role assigned to the user. This is a partial match filter.")]
        public string? RoleName { get; set; }

        [SwaggerSchema("PaginationInfo", Description = "Pagination information for the result set.")]
        public PaginationInfo? PaginationInfo { get; set; }

        [SwaggerSchema("OrderingInfo", Description = "Ordering information for the result set.")]
        public Dictionary<string, string>? OrderingInfo { get; set; }
    }
}

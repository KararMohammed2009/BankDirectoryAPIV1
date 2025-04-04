using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Enums;
using BankDirectoryApi.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Related.UserManagement
{
    public class UserFilterDTO
    {
        [Filter(FilterType.Contains, typeof(ApplicationUser), nameof(ApplicationUser.UserName))]
        public string UserName { get; set; }

        [Filter(FilterType.Contains, typeof(ApplicationUser), nameof(ApplicationUser.Email))]
        public string EmailAddress { get; set; }

        [Filter(FilterType.StartsWith, typeof(ApplicationUser), nameof(ApplicationUser.PhoneNumber))]
        public string PhoneNumber { get; set; }

        [Filter(FilterType.StartsWith, typeof(ApplicationRole), nameof(ApplicationRole.Name))]
        public string RoleName { get; set; }

        public PaginationInfo PaginationInfo { get; set; }
        public Dictionary<string, string> OrderingInfo { get; set; }
    }
}

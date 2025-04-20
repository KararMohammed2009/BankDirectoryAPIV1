using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Classes;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Core
{
    /// <summary>
    /// Data Transfer Object for filtering banks.
    /// </summary>
    public class BankFilterDTO
    {
        [Filter(FilterType.Contains, typeof(Bank), nameof(Bank.Name))]
        public string BankName { get; set; }

        [Filter(FilterType.Contains, typeof(Bank), nameof(Bank.Website))]
        public string BankWebsite { get; set; }

        [Filter(FilterType.StartsWith, typeof(Bank), nameof(Bank.Code))]
        public string BankCode { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public Dictionary<string,string> OrderingInfo { get; set; }
    }
}

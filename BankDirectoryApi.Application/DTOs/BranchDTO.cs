using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs
{
    public class BranchDTO
    {
        public int Id { get; set; }
        public int BankId { get; set; } // Foreign Key
        public string FullName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
    }
}

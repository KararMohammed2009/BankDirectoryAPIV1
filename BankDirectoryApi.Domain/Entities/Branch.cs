using BankDirectoryApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; }
        public int BankId { get; set; } // Foreign Key
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } 
        public string ContactNumber { get; set; } = string.Empty;

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

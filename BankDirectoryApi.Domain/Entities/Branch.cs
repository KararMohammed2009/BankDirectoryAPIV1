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
        public required string Name { get; set; }
        public Address? Address { get; set; } 
        public GeoCoordinate? GeoCoordinate { get; set; }
        public string? CustomerSupportNumber { get; set; }

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

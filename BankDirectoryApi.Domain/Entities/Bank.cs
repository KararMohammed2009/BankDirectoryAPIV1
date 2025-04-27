using BankDirectoryApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class Bank
    {
        public required int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string Code { get; set; } 
        public string? Website { get; set; }
        public string? CustomerSupportNumber { get; set; }
        public Address? Address { get; set; } 
        public GeoCoordinate? GeoCoordinate { get; set; } 

        // Navigation properties
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<ATM> ATMs { get; set; } = new List<ATM>();
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}

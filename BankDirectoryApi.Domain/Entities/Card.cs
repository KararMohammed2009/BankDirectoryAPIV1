using BankDirectoryApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class Card
    {
        public required int Id { get; set; }
        public required int BankId { get; set; } // Foreign Key
        public required string Name { get; set; } 
        public required CardType Type { get; set; }
        public decimal? AnnualFee { get; set; }
        public string? Description { get; set; }

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

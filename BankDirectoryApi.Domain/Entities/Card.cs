using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public int BankId { get; set; } // Foreign Key
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Debit, Credit, etc.
        public decimal AnnualFee { get; set; }

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

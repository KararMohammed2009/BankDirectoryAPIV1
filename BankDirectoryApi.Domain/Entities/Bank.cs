using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Unique bank code
        public string Website { get; set; } = string.Empty;
        public string CustomerSupportNumber { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<ATM> ATMs { get; set; } = new List<ATM>();
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}

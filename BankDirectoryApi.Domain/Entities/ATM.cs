using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class ATM
    {
        public int Id { get; set; }
        public int BankId { get; set; } // Foreign Key
        public string Location { get; set; } = string.Empty;
        public bool IsOperational { get; set; } = true;

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

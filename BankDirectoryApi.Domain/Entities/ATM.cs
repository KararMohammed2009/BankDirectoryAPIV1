using BankDirectoryApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Entities
{
    public class ATM
    {
        public required int Id { get; set; }
        public required int BankId { get; set; } // Foreign Key
        public required GeoCoordinate? GeoCoordinate { get; set; }
        public required bool IsOperational { get; set; }

        // Navigation Property
        public Bank Bank { get; set; } = null!;
    }
}

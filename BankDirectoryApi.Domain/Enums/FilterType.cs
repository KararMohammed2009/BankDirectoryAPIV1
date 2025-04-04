using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Enums
{
    public enum FilterType
    {
        Equals,       // Exact match
        Contains,     // String.Contains()
        StartsWith,   // String.StartsWith()
        EndsWith,     // String.EndsWith()
        GreaterThan,  // >=
        LessThan,     // <=
        GreaterThanOrEqual,  // >=
        LessThanOrEqual,     // <=
        NotEqual      // !=
        
    }
}

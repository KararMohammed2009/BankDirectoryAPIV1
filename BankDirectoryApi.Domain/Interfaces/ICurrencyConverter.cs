using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    //business rules, calculations, or validation logic
    //No data access, no logging, no API calls
    //If the logic is not tied to a single entity and is used across multiple parts of the system, it belongs in a Domain Service
    public interface ICurrencyConverter
    {
        decimal Convert(decimal amount, string fromCurrency, string toCurrency);
    }
}

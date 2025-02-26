using BankDirectoryApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Services
{
    public class CurrencyConverter:ICurrencyConverter
    {
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
                return amount; // No conversion needed

            if (fromCurrency == "USD" && toCurrency == "EUR")
                return amount * 0.92m; // Example rate

            if (fromCurrency == "EUR" && toCurrency == "USD")
                return amount * 1.09m; // Example rate

            throw new InvalidOperationException("Unsupported currency conversion.");
        }
    }
}

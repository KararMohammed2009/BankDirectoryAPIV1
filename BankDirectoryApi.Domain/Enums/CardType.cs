using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Enums
{
    public enum CardType
    {
        Debit = 1,
        Credit = 2,
        Prepaid = 3,
        Virtual = 4,
        Gift = 5,
        Business = 6
    }
}

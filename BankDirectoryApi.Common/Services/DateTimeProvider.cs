using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Services
{
    public class DateTimeProvider:IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

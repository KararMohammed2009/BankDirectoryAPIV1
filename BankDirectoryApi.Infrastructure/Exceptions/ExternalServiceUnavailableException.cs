using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Exceptions
{
    public class ExternalServiceUnavailableException:CustomExceptionBase
    {
        public ExternalServiceUnavailableException(string message = "External service is unavailable.")
           : base(message) { }
    }
}

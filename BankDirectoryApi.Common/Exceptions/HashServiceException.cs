using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public class HashServiceException:CustomExceptionBase
    {
        public HashServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public HashServiceException(string message) : base(message)
        {
        }
    }
}

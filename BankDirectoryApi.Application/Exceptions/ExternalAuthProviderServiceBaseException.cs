using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class ExternalAuthProviderServiceBaseException : CustomExceptionBase
    {
        public ExternalAuthProviderServiceBaseException(string message) : base(message)
        {
        }

        public ExternalAuthProviderServiceBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

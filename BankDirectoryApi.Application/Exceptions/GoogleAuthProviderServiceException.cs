using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class GoogleAuthProviderServiceException : CustomExceptionBase
    {
        public GoogleAuthProviderServiceException(string message) : base(message)
        {
        }

        public GoogleAuthProviderServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

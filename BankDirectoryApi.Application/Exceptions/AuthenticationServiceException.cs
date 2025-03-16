using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class AuthenticationServiceException :CustomExceptionBase
    {
        public AuthenticationServiceException(string message) : base(message)
        {
        }
        public AuthenticationServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class TokenHandlingException: CustomExceptionBase
    {
        public TokenHandlingException(string message) : base(message)
        {
        }
        public  TokenHandlingException(string message, Exception innerException):base(message, innerException)
        {
        }

    }
    
    
}

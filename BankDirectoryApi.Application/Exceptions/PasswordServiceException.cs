using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class PasswordServiceException:CustomExceptionBase
    {
        public PasswordServiceException(string message) : base(message)
        {
        }
        public PasswordServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

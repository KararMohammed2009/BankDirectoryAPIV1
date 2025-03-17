using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class EmailConfirmationServiceException:CustomExceptionBase
    {
        public EmailConfirmationServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EmailConfirmationServiceException(string message) : base(message)
        {
        }
    }
}

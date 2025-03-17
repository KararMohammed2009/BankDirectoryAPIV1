using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class UserServiceException :CustomExceptionBase
    {
        public UserServiceException(string message) : base(message)
        {
        }
        public UserServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

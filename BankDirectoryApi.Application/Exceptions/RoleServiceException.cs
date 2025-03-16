using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class RoleServiceException:CustomExceptionBase
    {
        public RoleServiceException(string message) : base(message)
        {
        }
        public RoleServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

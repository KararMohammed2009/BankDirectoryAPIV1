using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public class AuthorizationException: CustomExceptionBase
    {
        public AuthorizationException(string message = "Authorization Exception") : base(message)
        { }
    }
}

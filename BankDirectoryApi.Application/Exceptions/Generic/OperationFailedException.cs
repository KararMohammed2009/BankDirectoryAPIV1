using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions.Generic
{
    public class OperationFailedException:CustomExceptionBase
    {
        public OperationFailedException(string message) : base(message)
        {
        }
        public OperationFailedException(string message,Exception ex) : base(message,ex)
        {
        }

    }
}

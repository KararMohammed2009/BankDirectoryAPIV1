using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public class InvalidOperationCustomException:CustomExceptionBase
    {
        public InvalidOperationCustomException(string message) : base(message)
        {
        }
    }
}

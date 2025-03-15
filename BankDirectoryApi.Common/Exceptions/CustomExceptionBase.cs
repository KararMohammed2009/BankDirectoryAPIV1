using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public abstract class CustomExceptionBase : Exception
    {
        protected CustomExceptionBase(string message) : base(message)
        {

        }
        protected CustomExceptionBase(string message,Exception innerException) : base(message,innerException)
        {

        }
    }

}

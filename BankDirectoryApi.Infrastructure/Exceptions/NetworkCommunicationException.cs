using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Exceptions
{
    public class NetworkCommunicationException: CustomExceptionBase
    {
        public NetworkCommunicationException(string message = "Network communication error.")
            : base(message) { }
    }
    
}

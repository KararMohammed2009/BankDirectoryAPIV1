using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class JwtTokenGeneratorServiceException :CustomExceptionBase
    {
        public JwtTokenGeneratorServiceException(string message) : base(message)
        {
        }
        public JwtTokenGeneratorServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

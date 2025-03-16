using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class JwtTokenValidatorServiceException:CustomExceptionBase
    {
     
        public JwtTokenValidatorServiceException(string message) : base(message)
        {
        }
        public JwtTokenValidatorServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

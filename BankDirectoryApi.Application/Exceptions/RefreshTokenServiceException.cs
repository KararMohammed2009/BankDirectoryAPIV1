using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    class RefreshTokenServiceException: CustomExceptionBase
    {
        public RefreshTokenServiceException(string message) : base(message)
        {
        }
        public RefreshTokenServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}

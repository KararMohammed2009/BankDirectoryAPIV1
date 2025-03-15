using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public class ValidationException : CustomExceptionBase
    {
        public ValidationException(string message = "Validation Exception")
           : base(message) { }
        public ValidationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

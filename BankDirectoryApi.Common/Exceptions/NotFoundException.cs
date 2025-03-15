using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Exceptions
{
    public class NotFoundException : CustomExceptionBase
    {
        public NotFoundException(string message = "Not Found Exception")
            : base(message) { }
    }
}

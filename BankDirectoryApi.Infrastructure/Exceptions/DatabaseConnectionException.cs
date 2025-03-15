using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Exceptions
{
    public class DatabaseConnectionException: CustomExceptionBase
    {
        public DatabaseConnectionException(string message = "Database connection error.")
            : base(message) { }
    }
}

using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Exceptions
{
    public class FileStorageException:CustomExceptionBase
    {
        public FileStorageException(string message = "File storage error.")
            : base(message) { }
    }
}

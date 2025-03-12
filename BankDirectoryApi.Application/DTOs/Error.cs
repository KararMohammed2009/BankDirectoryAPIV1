using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs
{
    public class Error
    {
        public string? Code { get; set; }
        public required string Message { get; set; }
        public required Severity Severity { get; set; }
    }
}

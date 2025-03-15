using BankDirectoryApi.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Exceptions
{
    public class BusinessRuleViolationException:CustomExceptionBase
    {
        public BusinessRuleViolationException(string message = "Business Rule Violation Exception") 
            : base(message)
        {
        }
    }
}

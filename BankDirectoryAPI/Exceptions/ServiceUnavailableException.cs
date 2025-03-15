using BankDirectoryApi.Common.Exceptions;

namespace BankDirectoryApi.API.Exceptions
{
    public class ServiceUnavailableException:CustomExceptionBase
    {
        public ServiceUnavailableException(string message= "Service Unavailable Exception") : base(message)
        {
        }
    }
}

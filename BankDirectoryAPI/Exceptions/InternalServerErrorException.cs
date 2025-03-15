using BankDirectoryApi.Common.Exceptions;

namespace BankDirectoryApi.API.Exceptions
{
    public class InternalServerErrorException:CustomExceptionBase
    {
        public InternalServerErrorException(string message = "Internal Server Error Exception") 
            : base(message)
        {
        }
    }
}

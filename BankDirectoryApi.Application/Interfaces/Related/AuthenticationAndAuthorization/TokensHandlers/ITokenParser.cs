using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    public interface ITokenParser
    {
        string GetUserIdAsync(string accessToken);
    }
}

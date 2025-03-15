using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    public interface ITokenValidator
    {
        Task<bool> ValidateAccessTokenAsync(string accessToken);
    }
}

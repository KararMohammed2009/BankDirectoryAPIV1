using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    public interface IExternalAuthProvider
    {
        Task<(bool Success, IEnumerable<IdentityError>? errors, IdentityUser? User, AuthDTO? Response)> ManageExternalLogin(string code, ClientInfo clientInfo);
        string ProviderName { get; }
    }
}

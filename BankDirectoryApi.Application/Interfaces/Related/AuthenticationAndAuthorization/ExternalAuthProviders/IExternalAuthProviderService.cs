using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    public interface IExternalAuthProviderService
    {
        Task<(UserDTO userDTO,string externalAccessToken)>
             ManageExternalLogin(string code, ClientInfo clientInfo);
    }
}

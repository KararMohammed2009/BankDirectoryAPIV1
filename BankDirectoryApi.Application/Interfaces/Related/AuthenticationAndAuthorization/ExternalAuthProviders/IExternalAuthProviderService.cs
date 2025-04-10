using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    /// <summary>
    /// Interface for external authentication provider services.
    /// </summary>
    public interface IExternalAuthProviderService
    {
        /// <summary>
        /// Manages the external login process.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientInfo"></param>
        /// <returns> A tuple containing the user information and the external access token.</returns>
        Task<Result<(UserDTO userDTO,string externalAccessToken)>>
             ManageExternalLogin(string code, ClientInfo clientInfo);
    }
}

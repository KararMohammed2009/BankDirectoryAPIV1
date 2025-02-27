using BankDirectoryApi.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationDTO> RegisterAsync(RegisterRequest request);
        Task<AuthenticationDTO> LoginAsync(LoginRequest request);
        Task<AuthenticationDTO?> ExternalLoginAsync(string provider, string idToken);
    }
}

using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers
{
    public interface ITokenGeneratorService
    {
        Task<string> GenerateAccessTokenAsync(string userId, string userName, string email, IEnumerable<string> roles);

    }
}

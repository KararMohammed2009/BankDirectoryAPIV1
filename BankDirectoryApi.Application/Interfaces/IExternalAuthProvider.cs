using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IExternalAuthProvider
    {
        Task<(bool Success, User? User, AuthResponseDTO? Response)> ValidateAndGetUserAsync(string idToken);
        string GetProviderName();
    }
}


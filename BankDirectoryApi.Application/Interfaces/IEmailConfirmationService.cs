using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IEmailConfirmationService
    {
        public Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationDTO model);
        public Task<Result<bool>> ResendConfirmationEmailAsync(string email);

    }
}

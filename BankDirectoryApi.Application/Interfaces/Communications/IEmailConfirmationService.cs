using BankDirectoryApi.Application.DTOs.Communications;
using BankDirectoryApi.Application.DTOs.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Communications
{
    public interface IEmailConfirmationService
    {
        Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationDTO model);
        Task<Result<bool>> ResendConfirmationEmailAsync(string email);

    }
}

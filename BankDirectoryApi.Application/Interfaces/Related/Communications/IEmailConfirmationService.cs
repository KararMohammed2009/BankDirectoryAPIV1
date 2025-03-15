using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.Communications
{
    public interface IEmailConfirmationService
    {
        Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationDTO model);
        Task<Result<bool>> ResendConfirmationEmailAsync(string email);

    }
}

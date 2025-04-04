using BankDirectoryApi.Application.DTOs.Related.Communications;
using FluentResults;

namespace BankDirectoryApi.Application.Interfaces.Related.Communications
{
    public interface IEmailConfirmationService
    {
        Task<Result<string>> ConfirmEmailAsync(EmailConfirmationDTO model);
        Task<Result<string>> ResendConfirmationEmailAsync(string email);

    }
}

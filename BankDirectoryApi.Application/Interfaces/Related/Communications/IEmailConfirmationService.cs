using BankDirectoryApi.Application.DTOs.Related.Communications;

namespace BankDirectoryApi.Application.Interfaces.Related.Communications
{
    public interface IEmailConfirmationService
    {
        Task<bool> ConfirmEmailAsync(EmailConfirmationDTO model);
        Task<bool> ResendConfirmationEmailAsync(string email);

    }
}

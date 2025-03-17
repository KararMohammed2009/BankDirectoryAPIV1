using BankDirectoryApi.Application.DTOs.Related.Communications;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.Communications;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;

namespace BankDirectoryApi.Application.Services.Related.Communications
{
    public class EmailConfirmationService :IEmailConfirmationService
    {
        private readonly IUserService _userService;
        public EmailConfirmationService(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<bool> ConfirmEmailAsync(EmailConfirmationDTO model)
        {
            try
            {
                if (model == null) throw new Exception("Model is required");
                var result = await _userService.ConfirmEmailAsync(model.Email, model.Token);
                return result;
            }
            catch (Exception ex)
            {
                throw new EmailConfirmationServiceException("Email confirmation failed", ex);
            } 
        }

       public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                var token = await _userService.GenerateEmailConfirmationTokenAsync(email);
                // TODO: Send email with token to user email address 
                return true;
            }
            catch (Exception ex)
            {
                throw new EmailConfirmationServiceException("Resend confirmation email failed", ex);
            }
        }
    }
}

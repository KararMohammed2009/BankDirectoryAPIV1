using BankDirectoryApi.Application.DTOs.Related.Communications;
using BankDirectoryApi.Application.Interfaces.Related.Communications;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using FluentResults;
using System.Net;

namespace BankDirectoryApi.Application.Services.Related.Communications
{
    public class EmailConfirmationService :IEmailConfirmationService
    {
        private readonly IUserService _userService;
        public EmailConfirmationService(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<Result<string>> ConfirmEmailAsync(EmailConfirmationDTO model)
        {
           
                if (model == null) 
                return Result.Fail(new Error("model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var result = await _userService.ConfirmEmailAsync(model.Email, model.Token);
            if (result.IsFailed)
            {
                return Result.Fail(new Error("Confirm Email failed by UserService")
                 .WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(result.Errors);
            }
            return Result.Ok(model.Email);
        }

       public async Task<Result<string>> ResendConfirmationEmailAsync(string email)
        {
            
                if (string.IsNullOrEmpty(email))
                return Result.Fail(new Error("email is required")
                  .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var token = await _userService.GenerateEmailConfirmationTokenAsync(email);
            if (token.IsFailed)
            {
                return Result.Fail(new Error("Resend Confirmation Email failed by UserService")
                 .WithMetadata("StatusCode", HttpStatusCode.InternalServerError))
                    .WithErrors(token.Errors);
            }
                // TODO: Send email with token to user email address 
                return Result.Ok(email);
           
        }
    }
}

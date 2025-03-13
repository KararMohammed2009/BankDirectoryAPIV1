using BankDirectoryApi.Application.DTOs.Communications;
using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.Interfaces.Communications;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Communications
{
    public class EmailConfirmationService :IEmailConfirmationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public EmailConfirmationService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<bool>> ConfirmEmailAsync(EmailConfirmationDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Email Not Found", Severity = Severity.Error, Code = "EmailNotFound" } });

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            return result.Succeeded ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Email confirmation failed", Severity = Severity.Error, Code = "EmailConfirmationFailed" } });
        }

        public async Task<Result<bool>> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Result<bool>.FailureResult(
                new List<Error> { new Error { Message = "Email Not Found", Severity = Severity.Error, Code = "EmailNotFound" } });

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // TODO: Send token via email (implement email service)

            return Result<bool>.SuccessResult(true);
        }
    }
}

using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.Communications;
using BankDirectoryApi.Application.Interfaces.Related.Communications;
using BankDirectoryApi.Common.Exceptions;
using BankDirectoryApi.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.Communications
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
            if (user == null) throw new NotFoundException("User not found");
            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if(result.Succeeded) throw new InvalidOperationCustomException("Email confirmation failed");
            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> ResendConfirmationEmailAsync(string email)
        {
           
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new NotFoundException("User not found");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (token == null) throw new InvalidOperationCustomException("Token generation failed");
           
                // TODO: Send token via email (implement email service)
          

            return Result<bool>.SuccessResult(true);
        }
    }
}

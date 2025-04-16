using BankDirectoryApi.Application.Interfaces.Related.VerificationServices;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail.Model;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Services
{
    public class TwilioEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwilioEmailService> _logger;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        public TwilioEmailService(IConfiguration configuration,
            ILogger<TwilioEmailService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _apiKey = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_API_KEY",
                _configuration,
                "Email:Twilio:ApiKey",_logger).Value;
            _fromEmail = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_FROM_EMAIL",
                _configuration,
                "Email:Twilio:FromEmail",_logger).Value;
            _fromName = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_FROM_NAME",
                _configuration,
                "Email:Twilio:FromName",_logger).Value;
        }
        public async Task<Result> SendEmailAsync(string to, string subject, string body)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(to, nameof(to));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(subject, nameof(subject));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(body, nameof(body));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var toEmail = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent: null, body);
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to send email. Status code: {response.StatusCode}, Body: {await response.Body.ReadAsStringAsync()}");
                    return Result.Fail(new Error("Failed to send email using Twilio")
                        
                        .WithMetadata("ErrorCode", CommonErrors.OperationFailed));
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email using Twilio");
                return Result.Fail(new Error("Error sending email using Twilio").CausedBy(ex)
                    .WithMetadata("ErrorCode", CommonErrors.UnexpectedError));
            }

        }
    }
}

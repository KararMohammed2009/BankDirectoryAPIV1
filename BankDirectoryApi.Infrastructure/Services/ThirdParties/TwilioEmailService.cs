using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using Twilio;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties
{
    /// <summary>
    /// Service for sending emails using Twilio SendGrid.
    /// </summary>
    public class TwilioEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwilioEmailService> _logger;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly SendGridClient _sendGridClient;
        /// <summary>
        /// Constructor for TwilioEmailService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TwilioEmailService(IConfiguration configuration,
            ILogger<TwilioEmailService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _apiKey = SecureVariablesHelper.GetSecureVariable(
                "SENDGRID_API_KEY",
                _configuration,
                "Email:SendGrid:ApiKey",_logger).Value;
            _fromEmail = SecureVariablesHelper.GetSecureVariable(
                "SENDGRID_FROM_EMAIL",
                _configuration,
                "Email:SendGrid:FromEmail", _logger).Value;
            _fromName = SecureVariablesHelper.GetSecureVariable(
                "SENDGRID_FROM_NAME",
                _configuration,
                "Email:SendGrid:FromName", _logger).Value;
            _sendGridClient = new SendGridClient(_apiKey);
        }
        /// <summary>
        /// Sends an email asynchronously using Twilio SendGrid.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="plainTextContent"></param>
        /// <param name="htmlContent"></param>
        /// <returns>The result of the email sending operation.</returns>
        public async Task<Result> SendEmailAsync(string to,
            string subject,string plainTextContent, string htmlContent)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(to, nameof(to));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(subject, nameof(subject));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(htmlContent, nameof(htmlContent));
            var validationResultForPlainText = ValidationHelper.ValidateNullOrWhiteSpaceString(plainTextContent, nameof(plainTextContent));
            if(validationResult.IsFailed && validationResultForPlainText.IsFailed)
            {
                return Result.Fail(errors: 
                    validationResult.Errors.Concat(validationResultForPlainText.Errors).ToList());
            }
                
            try
            {
                var from = new EmailAddress(_fromEmail, _fromName);
                var toEmail = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);
                var message =await  response.Body.ReadAsStringAsync();
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
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }

        }
    }
}

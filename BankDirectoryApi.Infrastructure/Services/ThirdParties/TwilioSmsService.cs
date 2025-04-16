using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties
{
    /// <summary>
    /// Service for sending SMS using Twilio.
    /// </summary>
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _twilioFromNumber;
        private readonly ILogger<TwilioSmsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwilioSmsService"/> class.
        /// </summary>
        /// <param name="configration"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentException"></exception>
        public TwilioSmsService(
            IConfiguration configuration,
            ILogger<TwilioSmsService> logger
            )
        {
            _configuration = configuration;
            _twilioAccountSid = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_ACCOUNT_SID",
                _configuration,
                "Sms:Twilio:AccountSid",
                logger).Value;
            _twilioAuthToken = SecureVariablesHelper.GetSecureVariable("TWILIO_AUTH_TOKEN",
                _configuration,
                "Sms:Twilio:AuthToken",
                logger).Value;
            _twilioFromNumber = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_FROM_NUMBER",
                _configuration,
                "Sms:Twilio:FromNumber",
                logger).Value;
            
            _logger = logger;
        }
        /// <summary>
        /// Sends an SMS message to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns>The result of the SMS sending operation.</returns>
        public async Task<Result> SendSmsAsync(string phoneNumber, string message)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(phoneNumber);
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(message);
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);

            try
            {
                TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
                var theMessage = await MessageResource.CreateAsync(
               body: message,
               from: new PhoneNumber(_twilioFromNumber),
               to: new PhoneNumber(phoneNumber));
                if (theMessage.ErrorCode != null)
                {
                    _logger.LogError($"Failed to send SMS: {theMessage.ErrorMessage}");
                    return Result.Fail($"Failed to send SMS: {theMessage.ErrorMessage}");
                }


                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending SMS: {ex.Message}");
                return Result.Fail($"An error occurred while sending SMS: {ex.Message}");
            }
        }

    }
}

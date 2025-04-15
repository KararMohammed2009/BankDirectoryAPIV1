using BankDirectoryApi.Application.Interfaces.Related.VerificationServices;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BankDirectoryApi.Infrastructure.Services
{
    /// <summary>
    /// Service for sending SMS using Twilio.
    /// </summary>
    public class TwilloSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _twilioFromNumber;
        private readonly ILogger<TwilloSmsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwilloSmsService"/> class.
        /// </summary>
        /// <param name="configration"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentException"></exception>
        public TwilloSmsService(
            IConfiguration configration,
            ILogger<TwilloSmsService> logger
            )
        {
            _configuration = configration;
            _twilioAccountSid = _configuration["Sms:Twilio:AccountSid"]!;
            _twilioAuthToken = _configuration["Sms:Twilio:AuthToken"]!;
            _twilioFromNumber = _configuration["Sms:Twilio:FromNumber"]!;
            if (string.IsNullOrWhiteSpace(_twilioAccountSid) ||
                string.IsNullOrWhiteSpace(_twilioAuthToken) ||
                string.IsNullOrWhiteSpace(_twilioFromNumber))
            {
                throw new ArgumentException("Twilio configuration is missing.");
            }
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

using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties
{
    /// <summary>
    /// Service for sending SMS using Twilio.
    /// </summary>
    public class TwilioSmsService : ISmsService
    {

        private readonly ILogger<TwilioSmsService> _logger;
        private readonly ITwilioRestClient _twilioClient;
        private readonly string _fromNumber;

        /// <summary>
        /// Constructor for TwilioSmsService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="twilioClient"></param>
        /// <param name="twilioSettings"></param>
        public TwilioSmsService(
            ILogger<TwilioSmsService> logger,
            ITwilioRestClient twilioClient,
            TwilioSettings twilioSettings
            )
        {
            _logger = logger;
            _twilioClient = twilioClient;
            _fromNumber = twilioSettings.FromPhoneNumber!;
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
                var fromNumber = new PhoneNumber(_fromNumber);
                var toNumber = new PhoneNumber(phoneNumber);
                var theMessage = await MessageResource.CreateAsync(
               body: message,
               from: fromNumber,
               to: toNumber,
               client: _twilioClient);
                if (theMessage.ErrorCode != null)
                {
                    _logger.LogError($"Failed to send SMS: {theMessage.ErrorMessage}");
                    return Result.Fail($"Failed to send SMS: {theMessage.ErrorMessage}");
                }


                return Result.Ok();
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                if (ex.Code == 20404) // Invalid phone number
                {
                    return Result.Fail(new Error("Invalid phone number.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else if (ex.Code == 21608) // Phone number not verified
                {
                    return Result.Fail(new Error("Phone number not verified.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else
                {
                    _logger.LogError(ex, "Twilio error sending SMS to phone number: {PhoneNumber}", phoneNumber);
                    return Result.Fail(new Error("Failed to send SMS.")
                        .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
                return Result.Fail(new Error("Failed to send SMS.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }

    }
}

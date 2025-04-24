using BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Verify.V2.Service;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification
{
    /// <summary>
    /// Service for sending SMS verification codes using Twilio.
    /// </summary>
    public class TwilioSmsVerificationService : ISmsVerificationService
    {
        private readonly string _serviceSid;
        private readonly ILogger<TwilioSmsVerificationService> _logger;
        private readonly ITwilioRestClient _twilioClient;
        /// <summary>
        /// Constructor for TwilioSmsVerificationService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="twilioClient"></param>
        /// <param name="twilioSettings"></param>
        public TwilioSmsVerificationService(
            ILogger<TwilioSmsVerificationService> logger,
              ITwilioRestClient twilioClient, TwilioSettings twilioSettings)
        {


            _logger = logger;
            _twilioClient = twilioClient;
            _serviceSid = twilioSettings.VerificationServiceSid!;


        }
        /// <summary>
        /// Sends a verification code to the specified phone number using Twilio.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>The result of the verification code sending operation.</returns>
        public async Task<Result> SendVerificationCodeAsync(string phoneNumber)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(phoneNumber, nameof(phoneNumber));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            try
            {
                var verification = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: _serviceSid,
                    client: _twilioClient
                );

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
                    _logger.LogError(ex, "Error sending verification code.");
                    return Result.Fail(new Error("Error sending verification code.")
                        .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code.");
                return Result.Fail(new Error("Error sending verification code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }
        /// <summary>
        /// Verifies the code sent to the user's phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns>The result of the verification operation with a boolean indicating success or failure.</returns>
        public async Task<Result<bool>> VerifyCodeAsync(string phoneNumber, string code)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(phoneNumber, nameof(phoneNumber));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(code, nameof(code));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            try
            {
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: _serviceSid,
                    client: _twilioClient
                );
                if (verificationCheck.Status == "approved")
                {
                    return Result.Ok(true);
                }
                else
                {
                    return Result.Ok(false).WithSuccess(verificationCheck.Status);
                }
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                if (ex.Code == 20404) // Invalid verification code
                {
                    return Result.Fail(new Error("Invalid verification code.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else if (ex.Code == 21608) // Phone number not verified
                {
                    return Result.Fail(new Error("Phone number not verified.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else
                {
                    _logger.LogError(ex, "Error verifying code.");
                    return Result.Fail(new Error("Error verifying code.")
                        .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying code.");
                return Result.Fail(new Error("Error verifying code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }
    }
}

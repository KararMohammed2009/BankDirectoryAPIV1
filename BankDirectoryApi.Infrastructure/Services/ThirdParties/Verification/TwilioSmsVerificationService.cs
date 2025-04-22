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
    public class TwilioSmsVerificationService:ISmsVerificationService
    {
        private readonly IConfiguration _configuration;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _serviceSid;
        private readonly ILogger<TwilioSmsVerificationService> _logger;
        private readonly ITwilioRestClient _twilioClient;
        /// <summary>
        /// Constructor for TwilioSmsVerificationService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TwilioSmsVerificationService(IConfiguration configuration,
            ILogger<TwilioSmsVerificationService> logger,
              TwilioRestClient twilioClient)
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
            _serviceSid = SecureVariablesHelper.GetSecureVariable(
                "TWILIO_VERIFICATION_SERVICE_SID",
                _configuration,
                "Verification:Twilio:ServiceSid",
                logger).Value;

            _logger = logger;
            _twilioClient = twilioClient;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code.");
                return Result.Fail(new Error("Error sending verification code.")
                    .WithMetadata("ErrorCode",CommonErrors.ThirdPartyServiceError));
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying code.");
                return Result.Fail(new Error("Error verifying code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }
    }
}

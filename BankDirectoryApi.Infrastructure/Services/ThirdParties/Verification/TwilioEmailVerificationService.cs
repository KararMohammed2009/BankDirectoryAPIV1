using BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Verify.V2.Service;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification
{
    public class TwilioEmailVerificationService : IEmailVerificationService
    {
        private readonly IConfiguration _configuration;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _serviceSid;
        private readonly ILogger<TwilioEmailVerificationService> _logger;
        private readonly ITwilioRestClient _twilioClient;
        /// <summary>
        /// Constructor for TwilioEmailVerificationService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TwilioEmailVerificationService(IConfiguration configuration,
            ILogger<TwilioEmailVerificationService> logger,
            ITwilioRestClient twilioRestClient)
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
            _twilioClient = twilioRestClient;
        }
        /// <summary>
        /// Sends a verification code to the specified email address using Twilio.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The result of the verification code sending operation.</returns>
        public async Task<Result> SendVerificationCodeAsync(string email)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, nameof(email));
            if (validationResult.IsFailed)
                return (Result.Fail(validationResult.Errors));
            try
            {

                
                var verification = await VerificationResource.CreateAsync(
                   to: email,
                   channel: "email",
                   pathServiceSid: _serviceSid,
                   client: _twilioClient
               );
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code to email: {Email}", email);
                return Result.Fail(new Error("Failed to send verification code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }
        /// <summary>
        /// Verifies the provided code for the specified email address using Twilio.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>The result of the verification operation , true if the code is valid, false otherwise.</returns>
        public async Task<Result<bool>> VerifyCodeAsync(string email, string code)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, nameof(email));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(code, nameof(code));
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
            try
            {
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: email,
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
                _logger.LogError(ex, "Error verifying code for email: {Email}", email);
                return Result.Fail(new Error("Failed to verify code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }

    }
}

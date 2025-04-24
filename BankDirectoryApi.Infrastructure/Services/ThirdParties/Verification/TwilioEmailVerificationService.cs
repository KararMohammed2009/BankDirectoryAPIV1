using BankDirectoryApi.Application.Interfaces.Related.ThirdParties.Verification;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

using Twilio.Clients;
using Twilio.Rest.Verify.V2.Service;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification
{
    public class TwilioEmailVerificationService : IEmailVerificationService
    {
        private readonly string _serviceSid;
        private readonly ILogger<TwilioEmailVerificationService> _logger;
        private readonly ITwilioRestClient _twilioClient;


        /// <summary>
        /// Constructor for TwilioEmailVerificationService.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="twilioRestClient"></param>
        /// <param name="twilioSettings"></param>
        public TwilioEmailVerificationService(
            ILogger<TwilioEmailVerificationService> logger,
            ITwilioRestClient twilioRestClient,TwilioSettings twilioSettings)
        {
         
            _serviceSid = twilioSettings.VerificationServiceSid!;
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
            catch (Twilio.Exceptions.ApiException ex)
            {
                if (ex.Code == 20404) // Invalid email address
                {
                    return Result.Fail(new Error("Invalid email address.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else if (ex.Code == 20403) // Email address not verified
                {
                    return Result.Fail(new Error("Email address not verified.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else // Other Twilio API error
                {
                    _logger.LogError(ex, "Twilio error sending verification code to email: {Email}", email);
                    return Result.Fail(new Error("Failed to send verification code.")
                        .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
                }
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
        public async Task<Result<bool>> VerifyCodeAsync(string email, string code )
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
                    client: _twilioClient,
                    verificationSid: null



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
                if (ex.Code == 20404) // Invalid email address
                {
                    return Result.Fail(new Error("Invalid email address.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else if (ex.Code == 20403) // Email address not verified
                {
                    return Result.Fail(new Error("Email address not verified.")
                        .WithMetadata("ErrorCode", CommonErrors.InvalidInput));
                }
                else // Other Twilio API error
                {
                    _logger.LogError(ex, "Twilio error verifying code for email: {Email}", email);
                    return Result.Fail(new Error("Failed to verify code.")
                        .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
                }
            }

            catch (Exception ex) // General exception
            {
                _logger.LogError(ex, "Error verifying code for email: {Email}", email);
                return Result.Fail(new Error("Failed to verify code.")
                    .WithMetadata("ErrorCode", CommonErrors.ThirdPartyServiceError));
            }
        }

    }
}

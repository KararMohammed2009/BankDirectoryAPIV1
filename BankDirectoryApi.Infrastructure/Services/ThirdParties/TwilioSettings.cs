using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Services.ThirdParties
{
    /// <summary>
    /// Settings for Twilio services including SMS and email(SendGrid).
    /// </summary>
    public class TwilioSettings
    {
        public string? VerificationServiceSid { get; set; }
        public string? FromPhoneNumber { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }


    }
}

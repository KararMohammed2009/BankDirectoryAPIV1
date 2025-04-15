using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.VerificationServices
{
    /// <summary>
    /// Interface for SMS service.
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends an SMS message to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns>The result of the SMS sending operation.</returns>
        Task<Result> SendSmsAsync(string phoneNumber, string message);
    }
}

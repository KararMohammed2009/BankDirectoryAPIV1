using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.VerificationServices
{
    /// <summary>
    /// Interface for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns>The result of the email sending operation.</returns>
        Task<Result> SendEmailAsync(string to, string subject, string body);
    }
}

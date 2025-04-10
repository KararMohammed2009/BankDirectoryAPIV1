using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure
{
    public static class IdentityExceptionHelper
    {
        /// <summary>
        /// Helper class for handling exceptions related to external services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        /// <param name="operationName"></param>
        /// <returns>The result of the operation.</returns>
        public static async Task<T> Execute<T>(
            Func<Task<T>> action,
            ILogger logger,
            [CallerMemberName] string operationName = "")
        {
            try
            {
                return  await action();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, $"{operationName} - Invalid operation.");
                throw;
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex, $"{operationName} - Argument is null.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, $"{operationName} - Db Update exception.");
                throw;
            }
            
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationName} - Unhandled exception.");
                throw;
            }
        }
    }

}

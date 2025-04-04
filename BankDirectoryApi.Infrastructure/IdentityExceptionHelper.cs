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
        /// Executes an Identity operation and handle Exceptions Logging
        /// </summary>
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
}

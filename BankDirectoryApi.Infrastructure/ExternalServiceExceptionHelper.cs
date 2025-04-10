using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace BankDirectoryApi.Infrastructure
{
    /// <summary>
    /// Helper class for handling exceptions related to external services.
    /// </summary>
    public static class ExternalServiceExceptionHelper
    {
        /// <summary>
        /// Executes an external service operation and handles exceptions.
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
                return await action();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError($"An error occurred while executing {operationName}: {ex.Message}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError($"An error occurred while executing {operationName}: {ex.Message}");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                logger.LogError($"An error occurred while executing {operationName}: {ex.Message}");
                throw;
            }
            catch (UriFormatException ex) { 
                logger.LogError($"An error occurred while executing {operationName}: {ex.Message}");
                throw;
            }
         
            catch (Exception ex)
            {
                logger.LogError($"An unexpected error occurred while executing {operationName}: {ex.Message}");
                throw;
            }

        }
}
}

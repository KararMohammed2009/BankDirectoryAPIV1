using BankDirectoryApi.Common.Errors;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankDirectoryApi.Common.Helpers
{
    /// <summary>
    /// Helper class for managing secure variables in the application.
    /// </summary>
    public static class SecureVariablesHelper
    {
        /// <summary>
        /// Retrieves a secure variable from the environment or configuration.
        /// </summary>
        /// <param name="enviromentVariableName"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationVariableFullName"></param>
        /// <param name="logger"></param>
        /// <returns>The value of the secure variable.</returns>
        public static Result<string> GetSecureVariable(
            string enviromentVariableName,
            IConfiguration configuration,
            string configurationVariableFullName,ILogger logger)
        {
            var variableValue = Environment.GetEnvironmentVariable(enviromentVariableName);
            if (string.IsNullOrWhiteSpace(variableValue))
            {
                logger.LogWarning($"Environment variable '{enviromentVariableName}' is not set or is empty.");
                variableValue = configuration[configurationVariableFullName];
                if (string.IsNullOrWhiteSpace(variableValue))
                {
                    logger.LogWarning($"Configuration variable '{configurationVariableFullName}' is not set or is empty.");
                    return Result.Fail(new Error($"Configuration variable '{configurationVariableFullName}' is not set or is empty.")
                        .WithMetadata("ErrorCode",CommonErrors.ConfigurationError));
                }
            }
            return variableValue;
        }
    }
}

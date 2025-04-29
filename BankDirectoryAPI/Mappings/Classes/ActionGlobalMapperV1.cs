using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.API.Models;
using BankDirectoryApi.Common.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Mappings.Classes
{
    /// <summary>
    /// This class is responsible for mapping the results of application services to API responses.
    /// </summary>
    public class ActionGlobalMapperV1: IActionGlobalMapper
    {
        private readonly ILogger<ActionGlobalMapperV1> _logger;

        /// <summary>
        /// Constructor for ActionGlobalMapper.
        /// </summary>
        /// <param name="logger"></param>
        public ActionGlobalMapperV1(ILogger<ActionGlobalMapperV1> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Maps a Result object to an IActionResult.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns>The IActionResult representing the result of the operation.</returns>
        public IActionResult MapResultToApiResponse<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                var response = new ApiResponse<T>(result.Value, string.Empty, StatusCodes.Status200OK);

                return new OkObjectResult(response);
            }

            return HandleErrors(result.Errors);
        }
        /// <summary>
        /// Handles errors from a Result object and maps them to an IActionResult.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns>The IActionResult representing the error response.</returns>
        private IActionResult HandleErrors(List<IError> errors)
        {
            // Log the errors for debugging purposes
            foreach (var error in errors)
            {
                _logger.LogError(error.ToString());
            }
            int statusCode = StatusCodes.Status400BadRequest; // Default to Bad Request
            string errorMessage = "An error occurred."; // Generic user-friendly message

            if (errors.Any())
            {
                var firstError = errors.First();

                if (firstError.Metadata.TryGetValue("ErrorCode", out var errorCodeObj) && errorCodeObj is CommonErrors errorCode)
                {
                    switch (errorCode)
                    {
                        case CommonErrors.InvalidInput:
                        case CommonErrors.MissingRequiredField:
                        case CommonErrors.InvalidFormat:
                        case CommonErrors.OutOfRange:
                        case CommonErrors.NotUnique:
                            statusCode = StatusCodes.Status400BadRequest;
                            errorMessage = "Invalid input. Please check your data and try again.";
                            break;

                        case CommonErrors.ResourceNotFound:
                            statusCode = StatusCodes.Status404NotFound;
                            errorMessage = "Resource not found.";
                            break;

                        case CommonErrors.ResourceAlreadyExists:
                            statusCode = StatusCodes.Status409Conflict;
                            errorMessage = "Resource already exists.";
                            break;

                        case CommonErrors.DatabaseConnectionFailed:
                        case CommonErrors.DatabaseQueryFailed:
                        case CommonErrors.FileAccessDenied:
                        case CommonErrors.FileNotFound:
                        case CommonErrors.ExternalServiceUnavailable:
                        case CommonErrors.ExternalServiceTimeout:
                        case CommonErrors.ExternalServiceError:
                        case CommonErrors.IntegrationServiceUnavailable:
                        case CommonErrors.IntegrationTimeout:
                        case CommonErrors.NetworkError:
                        case CommonErrors.CacheFailure:
                        case CommonErrors.ServiceUnavailable:
                        case CommonErrors.Timeout:
                            statusCode = StatusCodes.Status503ServiceUnavailable;
                            errorMessage = "Service unavailable. Please try again later.";
                            break;

                        case CommonErrors.UnauthorizedAccess:
                        case CommonErrors.AuthenticationFailed:
                        case CommonErrors.InvalidToken:
                        case CommonErrors.ExpiredToken:
                            statusCode = StatusCodes.Status401Unauthorized;
                            errorMessage = "Unauthorized access. Please authenticate correctly.";
                            break;

                        case CommonErrors.Forbidden:
                            statusCode = StatusCodes.Status403Forbidden;
                            errorMessage = "Access forbidden.";
                            break;

                        case CommonErrors.BusinessRuleViolation:
                        case CommonErrors.OperationNotAllowed:
                            statusCode = StatusCodes.Status400BadRequest;
                            errorMessage = "Operation not allowed due to business rules.";
                            break;

                        case CommonErrors.InternalServerError:
                        case CommonErrors.NotImplemented:
                        case CommonErrors.ConfigurationError:
                        case CommonErrors.UnexpectedError:
                        case CommonErrors.OperationFailed:
                        case CommonErrors.ConcurrencyFailure:
                        case CommonErrors.LockAcquisitionFailure:
                        case CommonErrors.SerializationError:
                        case CommonErrors.DeserializationError:
                        case CommonErrors.IntegrationError:
                        case CommonErrors.SecurityViolation:
                        case CommonErrors.DataTampering:
                        case CommonErrors.CacheMiss:
                            statusCode = StatusCodes.Status500InternalServerError;
                            errorMessage = "An internal server error occurred. Please try again later.";
                            break;

                        default:
                            // Keep the generic message for unmapped errors
                            break;
                    }
                }
                else
                {
                    // Keep the generic message for errors without error codes
                }
            }

            var apiError = new ApiError { Message = errorMessage, Code = errors.Any() ? errors.First().GetType().Name : "BadRequest" };
            return new ObjectResult(new ApiResponse<object>(new List<ApiError> { apiError }, "Request Failed", statusCode)) { StatusCode = statusCode };
        }


    }

}

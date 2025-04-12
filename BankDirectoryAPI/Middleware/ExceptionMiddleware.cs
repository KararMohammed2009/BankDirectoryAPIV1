using BankDirectoryApi.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankDirectoryApi.API.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions globally in the application.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        /// <summary>
        /// Constructor for ExceptionMiddleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// Invokes the middleware to handle exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Task representing the asynchronous operation of the middleware.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing the request.");
                await HandleExceptionAsync(context);
            }
        }
        /// <summary>
        /// Handles the exception and creates a standardized error response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The serialized proper error response.</returns>
        private static async Task HandleExceptionAsync(HttpContext context)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var apiError = new ApiError
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = "Something went wrong. Please try again later."
            };

            var errorResponse = new ApiResponse<string>(
                errors: new List<ApiError> { apiError },
                message: "A server error occurred.",
                statusCode: statusCode
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }
    }
}

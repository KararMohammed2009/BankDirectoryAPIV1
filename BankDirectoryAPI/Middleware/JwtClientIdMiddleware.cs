using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace BankDirectoryApi.API.Middleware
{
    public class JwtClientIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtClientIdMiddleware> _logger;

        public JwtClientIdMiddleware(RequestDelegate next, ILogger<JwtClientIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extract User ID from JWT claims
            var userId = context.User?.FindFirst("sub")?.Value; // 'sub' is usually the user ID in JWT
            // Extract client IP and user-agent from the request
            var clientIp = context.Connection?.RemoteIpAddress?.ToString();
            var userAgent = context.Request?.Headers["User-Agent"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                // Store in HttpContext for rate limiting and logging
                context.Items["ClientId"] = userId;
            }
            else
            {
                // Default if no user found (e.g., unauthenticated users)
                context.Items["ClientId"] = "anonymous";
            }
            if (!string.IsNullOrEmpty(clientIp))
            {
                // Store in HttpContext for rate limiting and logging and other purposes
                context.Items["ClientIp"] = clientIp;
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                // Store in HttpContext for rate limiting and logging and other purposes
                context.Items["UserAgent"] = userAgent;
            }

            await _next(context);
        }
    }
}

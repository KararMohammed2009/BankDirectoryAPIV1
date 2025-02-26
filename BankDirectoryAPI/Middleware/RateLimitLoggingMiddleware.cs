using System.Text.Json;
namespace BankDirectoryApi.API.Middleware
{
    public class RateLimitLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitLoggingMiddleware> _logger;

        public RateLimitLoggingMiddleware(RequestDelegate next, ILogger<RateLimitLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 429) // Too Many Requests
            {
                var clientId = context.Request.Headers["X-Client-Id"].ToString() ?? "unknown";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var requestPath = context.Request.Path;
                var timestamp = DateTime.UtcNow.ToString("o");

                var logEntry = new
                {
                    Timestamp = timestamp,
                    ClientId = clientId,
                    IpAddress = ip,
                    Path = requestPath,
                    StatusCode = 429,
                    Message = "Rate limit exceeded"
                };

                _logger.LogWarning(JsonSerializer.Serialize(logEntry));
            }
        }
    }
}

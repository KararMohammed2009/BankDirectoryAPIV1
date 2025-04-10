using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace BankDirectoryApi.API.Middleware
{
    public class ClientParametersExtractorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ClientParametersExtractorMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ClientParametersExtractorMiddleware(RequestDelegate
            next, ILogger<ClientParametersExtractorMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _tokenParser = scope.ServiceProvider.GetRequiredService<ITokenParserService>();

                var accessToken = await context.GetTokenAsync("access_token");
                string? userId = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    userId = _tokenParser.GetUserIdAsync(accessToken).Value;
                }

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
}

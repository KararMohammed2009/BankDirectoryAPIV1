using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;

namespace BankDirectoryApi.API.Helpers
{
    public class ClientInfoHelper
    {
        public static ClientInfo GetClientInfo(HttpContext context)
        {
            return new ClientInfo
            {
                IpAddress = context.Items["ClientIp"]?.ToString() ?? string.Empty,
                UserAgent = context.Items["UserAgent"]?.ToString() ?? string.Empty,

            };
        }
    }
}

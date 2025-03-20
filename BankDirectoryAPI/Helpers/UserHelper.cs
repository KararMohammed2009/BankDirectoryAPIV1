using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;

namespace BankDirectoryApi.API.Helpers
{
    public class UserHelper
    {
        public static string GetUserId(HttpContext context)
        {
            return context.Items["ClientId"]?.ToString() ?? string.Empty;
        }
    }
}

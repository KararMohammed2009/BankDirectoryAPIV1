

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Represents information about the client making a request.
    /// </summary>
    public class ClientInfo
    {
        public string? IpAddress { get; set; } // Client's IP address
        public string? UserAgent { get; set; } // Client's user agent

    }
}

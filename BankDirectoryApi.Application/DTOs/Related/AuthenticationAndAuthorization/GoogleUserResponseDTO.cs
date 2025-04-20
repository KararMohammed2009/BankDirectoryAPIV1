

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// This class is used to deserialize the response from Google when a user logs in using Google authentication.
    /// </summary>
    public class GoogleUserResponseDTO
    {
        public string? Sub { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }

    }
}

namespace BankDirectoryApi.API.Models
{
    /// <summary>
    /// This class represents an error in the API response.
    /// </summary>
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}

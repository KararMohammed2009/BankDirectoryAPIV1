namespace BankDirectoryApi.API.Models
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}

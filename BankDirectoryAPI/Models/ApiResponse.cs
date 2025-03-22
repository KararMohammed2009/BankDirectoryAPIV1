namespace BankDirectoryApi.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<ApiError> Errors { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(T data, string message = null, int statusCode = 200)
        {
            Success = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        public ApiResponse(List<ApiError> errors, string message = null, int statusCode = 400)
        {
            Success = false;
            Errors = errors;
            Message = message;
            StatusCode = statusCode;
        }
    }
}

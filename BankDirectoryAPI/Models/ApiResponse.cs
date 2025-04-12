

namespace BankDirectoryApi.API.Models
{
    /// <summary>
    /// This class represents the structure of the API response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<ApiError> Errors { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        /// <summary>
        /// Constructor for ApiResponse indicating success.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        public ApiResponse(T data, string message = null, int statusCode = 200)
        {
            Success = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }
        /// <summary>
        /// Constructor for ApiResponse with errors indicating failure.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        public ApiResponse(List<ApiError> errors, string message = null, int statusCode = 400)
        {
            Success = false;
            Errors = errors;
            Message = message;
            StatusCode = statusCode;
        }
    }
}

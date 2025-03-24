using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Classes
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value => IsSuccess ? _value! : 
            throw new InvalidOperationException("Cannot access Value on failure.");
        public string? ErrorMessage { get; }
        public Exception? Exception { get; }

        private readonly T? _value;

        private Result(T value)
        {
            IsSuccess = true;
            _value = value;
            ErrorMessage = null;
            Exception = null;
        }

        private Result(string errorMessage, Exception? exception = null)
        {
            if (string.IsNullOrEmpty(errorMessage)) throw new ArgumentException("Error message cannot be empty.", nameof(errorMessage));
            IsSuccess = false;
            _value = default;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string errorMessage, Exception? exception = null)
            => new(errorMessage, exception);

        public T EnsureSuccess()
        {
            if (!IsSuccess)
                throw Exception ?? new Exception(ErrorMessage);
            return _value!;
        }

        public void Deconstruct(out bool isSuccess, out T? value, out string? errorMessage)
        {
            isSuccess = IsSuccess;
            value = _value;
            errorMessage = ErrorMessage;
        }
    }
}

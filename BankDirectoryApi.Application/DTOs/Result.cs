using BankDirectoryApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs
{
    public class Result<T>
    {
        public bool Success { get; }
        public T Data { get; }
        public List<Error> Errors { get; }

        private Result(T data)
        {
            Success = true;
            Data = data;
            Errors = new List<Error>();
        }

        private Result(List<Error> errors)
        {
            Success = false;
            Data = default!;
            Errors = errors ?? new List<Error>();
        }

        public static Result<T> SuccessResult(T data) => new(data);
        public static Result<T> FailureResult(List<Error> errors) => new(errors);
    }

}

    


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string? ErrorMessage { get; set; }
        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Value = value
            };
        }
        public static Result<T> Failure(string errorMessage, T value)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Value = value
            };
        }
    }
}


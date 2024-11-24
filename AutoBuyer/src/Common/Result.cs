using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.Common
{
    /// <summary>
    /// Result class - wrapping state and error
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, string error) 
        {
            if (isSuccess && string.IsNullOrEmpty(error) is false)
            {
                throw new InvalidOperationException("Only failed result can have an error");
            }
            if (isSuccess is false && string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("Failed result must have an error");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure { get => IsSuccess is false; }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value ,true, string.Empty);
        }
    }

    public class Result<T>: Result
    {
        private readonly T _value;

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (IsSuccess is false)
                {
                    throw new InvalidOperationException("Failed result must not have a value");
                }
                return _value;
            }
        }
    }
}

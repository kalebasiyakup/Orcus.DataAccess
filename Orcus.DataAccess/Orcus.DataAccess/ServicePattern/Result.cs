using System;
using System.Text;

namespace Orcus.DataAccess
{
    public enum ResultStatusCode
    {
        Ok = 200,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 500,
        ExistingItem = 600,
        Warning = 700,
        Info = 800
    }

    public class Result
    {
        public bool ResultStatus { get; set; }
        public ResultStatusCode ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public Exception Exception { get; set; }
        public bool HasError { get; set; }

        public Result()
        {
            ResultCode = ResultStatusCode.Ok;
            ResultMessage = ResultStatusCode.Ok.ToString();
            ResultStatus = true;
            HasError = false;
        }

        public Result(string message, bool hasError)
        {
            ResultCode = ResultStatusCode.InternalServerError;
            ResultMessage = message;
            ResultStatus = hasError;
            HasError = hasError;
        }

        public Result(Exception exception)
        {
            Exception = exception;

            StringBuilder builder = new StringBuilder();
            Exception iteration = exception;

            while (iteration != null)
            {
                builder.AppendLine($"{(string.IsNullOrEmpty(builder.ToString()) ? "" : " - ")} {iteration.Message}");
                iteration = iteration.InnerException;
            }

            ResultCode = ResultStatusCode.InternalServerError;
            ResultMessage = builder.ToString();
            ResultStatus = false;
            HasError = true;
        }
    }

    public class Result<T> : Result
    {
        public T ResultObject { get; set; }

        public Result()
        {
        }

        public Result(T data)
        {
            ResultObject = data;
        }

        public Result(T data, string message, bool hasError) : base(message, hasError)
        {
            ResultObject = data;
        }

        public Result(T data, Exception exception) : base(exception)
        {
            ResultObject = data;
        }

        public Result(Exception exception) : base(exception)
        {
        }
    }
}
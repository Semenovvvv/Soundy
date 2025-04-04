using Soundy.SharedLibrary.Enums;

namespace Soundy.SharedLibrary.Common.Response
{
    public class Response
    {
        public ResponseStatus Status { get; set; }
        public string? Message { get; set; }

        public static Response Success() => new() { Status = ResponseStatus.Success};
        public static Response Fail(ResponseStatus status, string message) => new() { Status = status, Message = message };
    }

    public class Response<T> : Response
    {
        public T? Data { get; set; }

        public static Response<T> Success(T data) => new() { Status = ResponseStatus.Success, Data = data };

        public new static Response<T> Fail(ResponseStatus status, string message) => new() { Status = status, Message = message };
    }
}

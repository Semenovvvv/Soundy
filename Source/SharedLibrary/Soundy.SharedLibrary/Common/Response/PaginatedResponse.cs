using Soundy.SharedLibrary.Enums;

namespace Soundy.SharedLibrary.Common.Response;

public class PaginatedResponse<T> : Response
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public static PaginatedResponse<T> Success(List<T> items, int total, int page, int size) => new()
    {
        Status = ResponseStatus.Success,
        Items = items,
        TotalCount = total,
        PageNumber = page,
        PageSize = size
    };

    public new static PaginatedResponse<T> Fail(ResponseStatus status, string message) => new()
    {
        Status = status,
        Message = message
    };
}


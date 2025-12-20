namespace testttt.Application.DTOs;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates an empty PaginatedResponse for a given page number and page size
    /// </summary>
    public static PaginatedResponse<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PaginatedResponse<T>
        {
            Data = new List<T>(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0
        };
    }

    /// <summary>
    /// Creates a PaginatedResponse from data with pagination parameters
    /// </summary>
    public static PaginatedResponse<T> Create(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        return new PaginatedResponse<T>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}


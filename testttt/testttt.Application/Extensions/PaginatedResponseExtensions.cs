using testttt.Application.DTOs;

namespace testttt.Application.Extensions;

/// <summary>
/// Extension methods for PaginatedResponse to provide common utility operations
/// </summary>
public static class PaginatedResponseExtensions
{
    /// <summary>
    /// Maps the data items in a PaginatedResponse to another type
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">The source PaginatedResponse</param>
    /// <param name="mapper">Mapping function</param>
    /// <returns>New PaginatedResponse with mapped data</returns>
    public static PaginatedResponse<TDestination> Map<TSource, TDestination>(
        this PaginatedResponse<TSource> source,
        Func<TSource, TDestination> mapper)
    {
        return new PaginatedResponse<TDestination>
        {
            Data = source.Data.Select(mapper),
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount
        };
    }

    /// <summary>
    /// Gets the starting item index for the current page (1-based)
    /// </summary>
    public static int GetStartIndex<T>(this PaginatedResponse<T> response)
    {
        if (response.TotalCount == 0)
            return 0;
        
        return (response.PageNumber - 1) * response.PageSize + 1;
    }

    /// <summary>
    /// Gets the ending item index for the current page (1-based)
    /// </summary>
    public static int GetEndIndex<T>(this PaginatedResponse<T> response)
    {
        var endIndex = response.PageNumber * response.PageSize;
        return Math.Min(endIndex, response.TotalCount);
    }

    /// <summary>
    /// Gets a formatted range string like "1-10 of 100"
    /// </summary>
    public static string GetRangeString<T>(this PaginatedResponse<T> response)
    {
        if (response.TotalCount == 0)
            return "0-0 of 0";
        
        var start = response.GetStartIndex();
        var end = response.GetEndIndex();
        return $"{start}-{end} of {response.TotalCount}";
    }

    /// <summary>
    /// Checks if the current page number is valid
    /// </summary>
    public static bool IsValidPage<T>(this PaginatedResponse<T> response)
    {
        return response.PageNumber >= 1 && 
               response.PageNumber <= response.TotalPages &&
               response.TotalPages > 0;
    }

    /// <summary>
    /// Checks if there are any items in the current page
    /// </summary>
    public static bool HasItems<T>(this PaginatedResponse<T> response)
    {
        return response.Data != null && response.Data.Any();
    }

    /// <summary>
    /// Gets the count of items in the current page
    /// </summary>
    public static int GetPageItemCount<T>(this PaginatedResponse<T> response)
    {
        return response.Data?.Count() ?? 0;
    }

    /// <summary>
    /// Creates a PaginatedResponse from a list with pagination parameters
    /// This is a helper method that can be used instead of manual object creation
    /// </summary>
    public static PaginatedResponse<T> ToPaginatedResponse<T>(
        this IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        return PaginatedResponse<T>.Create(data, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Creates a PaginatedResponse from a list by automatically calculating pagination
    /// </summary>
    public static PaginatedResponse<T> CreateFromList<T>(
        IEnumerable<T> allItems,
        int pageNumber,
        int pageSize)
    {
        var itemsList = allItems.ToList();
        var totalCount = itemsList.Count;
        
        var paginatedItems = itemsList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PaginatedResponse<T>
        {
            Data = paginatedItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Converts the data to a list
    /// </summary>
    public static List<T> ToList<T>(this PaginatedResponse<T> response)
    {
        return response.Data?.ToList() ?? new List<T>();
    }

    /// <summary>
    /// Converts the data to an array
    /// </summary>
    public static T[] ToArray<T>(this PaginatedResponse<T> response)
    {
        return response.Data?.ToArray() ?? Array.Empty<T>();
    }

    /// <summary>
    /// Gets metadata about the pagination state
    /// </summary>
    public static PaginationMetadata GetMetadata<T>(this PaginatedResponse<T> response)
    {
        return new PaginationMetadata
        {
            CurrentPage = response.PageNumber,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            TotalPages = response.TotalPages,
            HasPreviousPage = response.HasPreviousPage,
            HasNextPage = response.HasNextPage,
            StartIndex = response.GetStartIndex(),
            EndIndex = response.GetEndIndex(),
            ItemCount = response.GetPageItemCount()
        };
    }
}

/// <summary>
/// Metadata about pagination state
/// </summary>
public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public int ItemCount { get; set; }
}


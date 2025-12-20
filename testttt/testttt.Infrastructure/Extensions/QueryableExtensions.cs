using Microsoft.EntityFrameworkCore;

namespace testttt.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IQueryable to provide pagination functionality
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to an IQueryable and returns the paginated results along with total count
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable source</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A tuple containing the paginated items and total count</returns>
    public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        // Validate and normalize pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination: skip and take
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Applies pagination to an IQueryable with ordering
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable source</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="orderBy">The ordering expression</param>
    /// <param name="descending">Whether to order descending (default: false)</param>
    /// <returns>A tuple containing the paginated items and total count</returns>
    public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPaginatedAsync<T, TKey>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        System.Linq.Expressions.Expression<Func<T, TKey>> orderBy,
        bool descending = false)
    {
        // Validate and normalize pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Apply ordering
        var orderedQuery = descending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        // Get total count before pagination
        var totalCount = await orderedQuery.CountAsync();

        // Apply pagination: skip and take
        var items = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Applies pagination to an IQueryable with multiple ordering
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable source</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="orderBy">The primary ordering expression</param>
    /// <param name="thenBy">The secondary ordering expression</param>
    /// <param name="primaryDescending">Whether primary order is descending (default: false)</param>
    /// <param name="secondaryDescending">Whether secondary order is descending (default: false)</param>
    /// <returns>A tuple containing the paginated items and total count</returns>
    public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPaginatedAsync<T, TKey1, TKey2>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        System.Linq.Expressions.Expression<Func<T, TKey1>> orderBy,
        System.Linq.Expressions.Expression<Func<T, TKey2>> thenBy,
        bool primaryDescending = false,
        bool secondaryDescending = false)
    {
        // Validate and normalize pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Apply ordering
        var orderedQuery = primaryDescending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        orderedQuery = secondaryDescending
            ? ((IOrderedQueryable<T>)orderedQuery).ThenByDescending(thenBy)
            : ((IOrderedQueryable<T>)orderedQuery).ThenBy(thenBy);

        // Get total count before pagination
        var totalCount = await orderedQuery.CountAsync();

        // Apply pagination: skip and take
        var items = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}


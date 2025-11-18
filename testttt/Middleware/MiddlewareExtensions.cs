namespace testttt.Middleware;

/// <summary>
/// Extension methods برای افزودن راحت middlewareها
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// افزودن SimpleLoggingMiddleware
    /// </summary>
    public static IApplicationBuilder UseSimpleLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SimpleLoggingMiddleware>();
    }

    /// <summary>
    /// افزودن RequestTimingMiddleware
    /// </summary>
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimingMiddleware>();
    }

    /// <summary>
    /// افزودن CustomHeaderMiddleware
    /// </summary>
    public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomHeaderMiddleware>();
    }

    /// <summary>
    /// افزودن RequestResponseLoggingMiddleware
    /// </summary>
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }

    /// <summary>
    /// افزودن GlobalExceptionMiddleware
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }

    /// <summary>
    /// افزودن RateLimitingMiddleware
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}


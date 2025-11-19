using System.Collections.Concurrent;

namespace testttt.Presentation.Middleware;

/// <summary>
/// سطح 6: پیشرفته - محدود کردن تعداد درخواست (Rate Limiting)
/// این middleware تعداد درخواست‌های هر IP را محدود می‌کند
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    // دیکشنری برای ذخیره تعداد درخواست‌های هر IP
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _requestCounts = new();
    
    // تنظیمات Rate Limiting
    private readonly int _maxRequests = 100; // حداکثر 100 درخواست (افزایش یافت برای development)
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1); // در هر دقیقه

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // بررسی Rate Limit
        if (!IsAllowed(clientIp))
        {
            _logger.LogWarning($"Rate limit exceeded for IP: {clientIp}");
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            
            // اضافه کردن CORS headers برای جلوگیری از CORS error
            // این headers باید قبل از نوشتن response اضافه شوند
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Methods"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            }
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Headers"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            }
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "تعداد درخواست‌های شما بیش از حد مجاز است. لطفا کمی صبر کنید.",
                retryAfter = 60 // ثانیه
            });
            
            return;
        }

        await _next(context);
    }

    private bool IsAllowed(string clientIp)
    {
        var now = DateTime.UtcNow;
        var key = clientIp;

        // دریافت یا ایجاد اطلاعات Rate Limit برای این IP
        var rateLimitInfo = _requestCounts.AddOrUpdate(
            key,
            new RateLimitInfo { Count = 1, ResetTime = now.Add(_timeWindow) },
            (k, existing) =>
            {
                // اگر زمان reset گذشته باشد، از نو شروع کن
                if (existing.ResetTime < now)
                {
                    return new RateLimitInfo { Count = 1, ResetTime = now.Add(_timeWindow) };
                }

                // افزایش تعداد درخواست
                existing.Count++;
                return existing;
            }
        );

        // پاکسازی IPهای قدیمی (هر 5 دقیقه یکبار)
        if (DateTime.UtcNow.Minute % 5 == 0)
        {
            CleanupOldEntries();
        }

        return rateLimitInfo.Count <= _maxRequests;
    }

    private void CleanupOldEntries()
    {
        var now = DateTime.UtcNow;
        var keysToRemove = _requestCounts
            .Where(kvp => kvp.Value.ResetTime < now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requestCounts.TryRemove(key, out _);
        }
    }

    private class RateLimitInfo
    {
        public int Count { get; set; }
        public DateTime ResetTime { get; set; }
    }
}


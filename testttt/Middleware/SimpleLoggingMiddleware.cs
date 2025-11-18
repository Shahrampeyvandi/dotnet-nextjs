namespace testttt.Middleware;

/// <summary>
/// سطح 1: ساده ترین middleware - فقط لاگ کردن درخواست
/// این middleware فقط مسیر درخواست را در کنسول نمایش می‌دهد
/// </summary>
public class SimpleLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // قبل از پردازش درخواست
        Console.WriteLine($"[SimpleLogging] درخواست دریافت شد: {context.Request.Path}");

        // فراخوانی middleware بعدی در pipeline
        await _next(context);

        // بعد از پردازش درخواست
        Console.WriteLine($"[SimpleLogging] پاسخ ارسال شد: {context.Response.StatusCode}");
    }
}


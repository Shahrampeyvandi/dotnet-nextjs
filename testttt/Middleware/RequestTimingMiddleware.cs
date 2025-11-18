namespace testttt.Middleware;

/// <summary>
/// سطح 2: اندازه گیری زمان پاسخ
/// این middleware زمان پردازش هر درخواست را محاسبه و در هدر پاسخ قرار می‌دهد
/// </summary>
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // شروع تایمر
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // پردازش درخواست
        await _next(context);

        // توقف تایمر
        stopwatch.Stop();

        // افزودن زمان پردازش به هدر پاسخ
        context.Response.Headers.Add("X-Response-Time", $"{stopwatch.ElapsedMilliseconds}ms");
        
        Console.WriteLine($"[RequestTiming] مسیر: {context.Request.Path} - زمان: {stopwatch.ElapsedMilliseconds}ms");
    }
}


namespace testttt.Middleware;

/// <summary>
/// سطح 3: افزودن هدرهای سفارشی
/// این middleware هدرهای سفارشی به پاسخ اضافه می‌کند
/// </summary>
public class CustomHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public CustomHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // افزودن هدرهای سفارشی قبل از پردازش
        context.Response.Headers.Add("X-Powered-By", "ASP.NET Core Custom Middleware");
        context.Response.Headers.Add("X-Server-Name", Environment.MachineName);
        context.Response.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());

        await _next(context);
    }
}


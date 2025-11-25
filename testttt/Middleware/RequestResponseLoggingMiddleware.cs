namespace testttt.Middleware;

/// <summary>
/// سطح 4: لاگ کامل درخواست و پاسخ
/// این middleware جزئیات کامل درخواست و پاسخ را لاگ می‌کند
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ذخیره body اصلی برای خواندن مجدد
        var originalBodyStream = context.Response.Body;

        // لاگ اطلاعات درخواست
        _logger.LogInformation("=== درخواست ورودی ===");
        _logger.LogInformation($"Method: {context.Request.Method}");
        _logger.LogInformation($"Path: {context.Request.Path}");
        _logger.LogInformation($"QueryString: {context.Request.QueryString}");
        _logger.LogInformation($"IP: {context.Connection.RemoteIpAddress}");

        // خواندن body درخواست (اگر وجود داشته باشد)
        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            _logger.LogInformation($"Request Body: {requestBody}");
        }

        // استفاده از MemoryStream برای ذخیره پاسخ
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // لاگ اطلاعات پاسخ
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation("=== پاسخ خروجی ===");
        _logger.LogInformation($"Status Code: {context.Response.StatusCode}");
        
        // محدود کردن اندازه Response Body برای جلوگیری از لاگ‌های خیلی بزرگ
        // (مثلاً HTML صفحات Swagger می‌توانند خیلی بزرگ باشند)
        const int maxLogLength = 10000; // حداکثر 10000 کاراکتر
        if (responseText.Length > maxLogLength)
        {
            _logger.LogInformation($"Response Body (truncated to {maxLogLength} chars): {responseText.Substring(0, maxLogLength)}... [Total length: {responseText.Length} chars]");
        }
        else
        {
            _logger.LogInformation($"Response Body: {responseText}");
        }

        // کپی کردن پاسخ به stream اصلی
        await responseBody.CopyToAsync(originalBodyStream);
    }
}


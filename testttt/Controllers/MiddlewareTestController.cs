using Microsoft.AspNetCore.Mvc;

namespace testttt.Controllers;

/// <summary>
/// Controller برای تست middlewareهای مختلف
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MiddlewareTestController : ControllerBase
{
    private readonly ILogger<MiddlewareTestController> _logger;

    public MiddlewareTestController(ILogger<MiddlewareTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// تست ساده - برای مشاهده لاگ و timing
    /// GET: api/MiddlewareTest/simple
    /// </summary>
    [HttpGet("simple")]
    public IActionResult SimpleTest()
    {
        return Ok(new
        {
            message = "تست ساده موفق بود!",
            timestamp = DateTime.Now,
            description = "این endpoint برای تست SimpleLogging و RequestTiming است"
        });
    }

    /// <summary>
    /// تست با Body - برای مشاهده RequestResponseLogging
    /// POST: api/MiddlewareTest/with-body
    /// </summary>
    [HttpPost("with-body")]
    public IActionResult TestWithBody([FromBody] TestRequest request)
    {
        return Ok(new
        {
            message = "درخواست با body دریافت شد",
            receivedData = request,
            processedAt = DateTime.Now,
            description = "این endpoint برای تست RequestResponseLoggingMiddleware است"
        });
    }

    /// <summary>
    /// تست خطا - برای مشاهده GlobalExceptionMiddleware
    /// GET: api/MiddlewareTest/error
    /// </summary>
    [HttpGet("error")]
    public IActionResult TestError()
    {
        throw new Exception("این یک خطای تستی است! GlobalExceptionMiddleware باید این را catch کند.");
    }

    /// <summary>
    /// تست خطای خاص - برای مشاهده انواع مختلف خطا
    /// GET: api/MiddlewareTest/error/{type}
    /// </summary>
    [HttpGet("error/{type}")]
    public IActionResult TestSpecificError(string type)
    {
        return type.ToLower() switch
        {
            "nullreference" => throw new NullReferenceException("NullReferenceException تستی"),
            "argument" => throw new ArgumentException("ArgumentException تستی"),
            "unauthorized" => throw new UnauthorizedAccessException("UnauthorizedAccessException تستی"),
            _ => throw new Exception($"خطای تستی از نوع: {type}")
        };
    }

    /// <summary>
    /// تست Rate Limiting - این endpoint را چندین بار سریع فراخوانی کنید
    /// GET: api/MiddlewareTest/rate-limit
    /// </summary>
    [HttpGet("rate-limit")]
    public IActionResult TestRateLimit()
    {
        return Ok(new
        {
            message = "درخواست موفق بود!",
            requestNumber = Random.Shared.Next(1, 1000),
            timestamp = DateTime.Now,
            description = "این endpoint را 10+ بار سریع فراخوانی کنید تا Rate Limiting را ببینید"
        });
    }

    /// <summary>
    /// تست Delay - برای مشاهده timing در middleware
    /// GET: api/MiddlewareTest/delay?seconds=2
    /// </summary>
    [HttpGet("delay")]
    public async Task<IActionResult> TestDelay([FromQuery] int seconds = 1)
    {
        if (seconds > 10) seconds = 10; // محدود کردن به 10 ثانیه
        
        await Task.Delay(seconds * 1000);
        
        return Ok(new
        {
            message = $"پاسخ بعد از {seconds} ثانیه",
            delaySeconds = seconds,
            timestamp = DateTime.Now,
            description = "این endpoint برای تست RequestTimingMiddleware است"
        });
    }

    /// <summary>
    /// تست Headers - برای مشاهده CustomHeaderMiddleware
    /// GET: api/MiddlewareTest/headers
    /// </summary>
    [HttpGet("headers")]
    public IActionResult TestHeaders()
    {
        var customHeaders = new Dictionary<string, string>();
        
        // خواندن هدرهای سفارشی که middleware اضافه کرده
        if (Response.Headers.ContainsKey("X-Powered-By"))
            customHeaders["X-Powered-By"] = Response.Headers["X-Powered-By"].ToString();
        
        if (Response.Headers.ContainsKey("X-Server-Name"))
            customHeaders["X-Server-Name"] = Response.Headers["X-Server-Name"].ToString();
        
        if (Response.Headers.ContainsKey("X-Request-ID"))
            customHeaders["X-Request-ID"] = Response.Headers["X-Request-ID"].ToString();
        
        if (Response.Headers.ContainsKey("X-Response-Time"))
            customHeaders["X-Response-Time"] = Response.Headers["X-Response-Time"].ToString();

        return Ok(new
        {
            message = "هدرهای سفارشی را بررسی کنید",
            customHeaders = customHeaders,
            allRequestHeaders = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            description = "این endpoint برای تست CustomHeaderMiddleware و RequestTimingMiddleware است"
        });
    }

    /// <summary>
    /// تست کامل - همه middlewareها را با هم تست می‌کند
    /// POST: api/MiddlewareTest/full-test
    /// </summary>
    [HttpPost("full-test")]
    public async Task<IActionResult> FullTest([FromBody] TestRequest request)
    {
        // کمی delay برای مشاهده timing
        await Task.Delay(500);
        
        return Ok(new
        {
            message = "تست کامل موفق بود!",
            receivedData = request,
            timestamp = DateTime.Now,
            description = "این endpoint همه middlewareها را با هم تست می‌کند",
            middlewareTested = new[]
            {
                "SimpleLoggingMiddleware",
                "RequestTimingMiddleware",
                "CustomHeaderMiddleware",
                "RequestResponseLoggingMiddleware",
                "GlobalExceptionMiddleware",
                "RateLimitingMiddleware"
            }
        });
    }
}

/// <summary>
/// مدل برای تست Body
/// </summary>
public class TestRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public Dictionary<string, object>? AdditionalData { get; set; }
}


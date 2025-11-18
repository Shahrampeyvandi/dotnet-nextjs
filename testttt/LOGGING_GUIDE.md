# راهنمای مشاهده لاگ‌های Middleware

## 📍 محل نمایش لاگ‌ها

### 1. Console.WriteLine (لاگ‌های ساده)
**Middlewareهایی که از `Console.WriteLine` استفاده می‌کنند:**
- `SimpleLoggingMiddleware`
- `RequestTimingMiddleware`

**محل نمایش:**
- ✅ **کنسول/ترمینال** که پروژه را اجرا کرده‌اید
- ✅ **Output Window** در Visual Studio (View → Output → Show output from: Debug)
- ✅ **Debug Console** در VS Code

**مثال خروجی:**
```
[SimpleLogging] درخواست دریافت شد: /api/MiddlewareTest/simple
[RequestTiming] مسیر: /api/MiddlewareTest/simple - زمان: 15ms
[SimpleLogging] پاسخ ارسال شد: 200
```

---

### 2. ILogger (لاگ‌های پیشرفته)
**Middlewareهایی که از `ILogger` استفاده می‌کنند:**
- `RequestResponseLoggingMiddleware`
- `GlobalExceptionMiddleware`
- `RateLimitingMiddleware`

**محل نمایش:**
- ✅ **کنسول/ترمینال** (پیش‌فرض)
- ✅ **Output Window** در Visual Studio
- ✅ **Debug Console** در VS Code
- ✅ می‌توانید به فایل هم اضافه کنید (نیاز به تنظیمات اضافی)

**مثال خروجی:**
```
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      === درخواست ورودی ===
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Method: GET
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Path: /api/MiddlewareTest/simple
```

---

## 🔍 نحوه مشاهده لاگ‌ها

### در Visual Studio:
1. پروژه را اجرا کنید (F5)
2. به **View → Output** بروید
3. در dropdown بالا، **"Debug"** را انتخاب کنید
4. درخواست به API بفرستید
5. لاگ‌ها را در Output Window ببینید

### در VS Code:
1. پروژه را اجرا کنید (F5 یا `dotnet run`)
2. به **Terminal** یا **Debug Console** بروید
3. لاگ‌ها را در همانجا ببینید

### در Command Line:
1. پروژه را با `dotnet run` اجرا کنید
2. لاگ‌ها مستقیماً در همان terminal نمایش داده می‌شوند

---

## 🎯 تست لاگ‌ها

### تست Console.WriteLine:
```bash
GET /api/MiddlewareTest/simple
```
**خروجی در کنسول:**
```
[SimpleLogging] درخواست دریافت شد: /api/MiddlewareTest/simple
[RequestTiming] مسیر: /api/MiddlewareTest/simple - زمان: 12ms
[SimpleLogging] پاسخ ارسال شد: 200
```

### تست ILogger:
```bash
POST /api/MiddlewareTest/with-body
Body: {"name":"علی","age":25}
```
**خروجی در کنسول:**
```
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      === درخواست ورودی ===
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Method: POST
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Path: /api/MiddlewareTest/with-body
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Request Body: {"name":"علی","age":25}
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      === پاسخ خروجی ===
info: testttt.Middleware.RequestResponseLoggingMiddleware[0]
      Status Code: 200
```

### تست خطا:
```bash
GET /api/MiddlewareTest/error
```
**خروجی در کنسول:**
```
fail: testttt.Middleware.GlobalExceptionMiddleware[0]
      خطای غیرمنتظره رخ داد: این یک خطای تستی است!
      System.Exception: این یک خطای تستی است!
         at testttt.Controllers.MiddlewareTestController.TestError()...
```

---

## ⚙️ تنظیمات لاگ

فایل `appsettings.json` و `appsettings.Development.json` تنظیمات لاگ را کنترل می‌کنند:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "testttt.Middleware": "Information"  // سطح لاگ برای middlewareها
    }
  }
}
```

**سطح‌های لاگ:**
- `Trace` - جزئی‌ترین
- `Debug` - برای دیباگ
- `Information` - اطلاعات عمومی (پیش‌فرض)
- `Warning` - هشدار
- `Error` - خطا
- `Critical` - بحرانی

---

## 💡 نکات مهم

1. **ترتیب middlewareها مهم است** - لاگ‌ها به ترتیب اجرای middleware نمایش داده می‌شوند
2. **Console.WriteLine** سریع‌تر است اما کمتر قابل تنظیم
3. **ILogger** انعطاف‌پذیرتر است و می‌توانید به فایل، دیتابیس، و غیره هم بفرستید
4. در **Production** بهتر است از `ILogger` استفاده کنید و `Console.WriteLine` را حذف کنید


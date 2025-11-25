using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using testttt.Application.Interfaces;
using testttt.Application.Services;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;
using testttt.Infrastructure.Repositories;
using testttt.Presentation.Middleware;
using Log = Serilog.Log;

// فعال کردن SelfLog برای مشاهده خطاهای Serilog
Serilog.Debugging.SelfLog.Enable(Console.Error);

// Configure Serilog قبل از ایجاد builder
var tempConfig = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(tempConfig)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();


Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging - read from configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    // اضافه کردن MSSqlServer sink با فیلتر برای حذف لاگ‌های Microsoft.AspNetCore از دیتابیس
    // لاگ‌های Microsoft.AspNetCore فقط در Console نمایش داده می‌شوند، نه در دیتابیس
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent =>
        {
            // فیلتر کردن لاگ‌های Microsoft.AspNetCore از دیتابیس
            if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
            {
                var sourceContextValue = sourceContext.ToString();
                // حذف کوتیشن‌ها از ابتدا و انتها (Serilog ممکن است آنها را اضافه کند)
                var cleanValue = sourceContextValue.Trim('"', '\'');
                return cleanValue.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        })
        .WriteTo.MSSqlServer(
            connectionString: context.Configuration.GetConnectionString("DefaultConnection") ??
                             "Server=.;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                SchemaName = "dbo",
                AutoCreateSqlTable = false, // چون در Program.cs ایجاد می‌کنیم
                BatchPostingLimit = 1,
                EagerlyEmitFirstEvent = true // برای اطمینان از نوشتن فوری اولین لاگ
            },
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)));

// Add services to the container.

// Add Entity Framework with Identity
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // SignIn settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ECommerceDbContext>()
.AddDefaultTokenProviders();

// Configure Identity Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.Cookie.Path = "/";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/api/Auth/login";
    options.LogoutPath = "/api/Auth/logout";
});

// Register Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// IUserRepository removed - using Identity UserManager instead

// Register Unit of Work
builder.Services.AddScoped<testttt.Application.Interfaces.IUnitOfWork, testttt.Infrastructure.Repositories.UnitOfWork>();

// Register Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// IUserService removed - using Identity UserManager and SignInManager instead

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // برای CORS
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // در Development از HTTP هم کار می‌کند
    options.Cookie.Name = ".AspNetCore.Session"; // نام cookie session
    options.Cookie.Path = "/";
    // مهم: Cookie باید همیشه set شود، حتی اگر session تغییر نکرده باشد
    // این باعث می‌شود که session در درخواست‌های بعدی حفظ شود
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");

        // بررسی و ایجاد جدول Logs در صورت عدم وجود (ساختار استاندارد Serilog MSSqlServer)
        try
        {
            // بررسی وجود جدول
            var tableExists = await context.Database.ExecuteSqlRawAsync(@"
                SELECT CASE 
                    WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
                    THEN 1 ELSE 0 END");

            // اگر جدول وجود ندارد، ایجاد کن
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Logs] (
                        [Id] INT IDENTITY(1,1) NOT NULL,
                        [Message] NVARCHAR(MAX) NULL,
                        [MessageTemplate] NVARCHAR(MAX) NULL,
                        [Level] NVARCHAR(128) NULL,
                        [TimeStamp] DATETIME2 NOT NULL,
                        [Exception] NVARCHAR(MAX) NULL,
                        [Properties] NVARCHAR(MAX) NULL,
                        [LogEvent] NVARCHAR(MAX) NULL,
                        CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)
                    );
                    
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Logs_TimeStamp' AND object_id = OBJECT_ID(N'[dbo].[Logs]'))
                        CREATE NONCLUSTERED INDEX [IX_Logs_TimeStamp] ON [dbo].[Logs] ([TimeStamp] DESC);
                    
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Logs_Level' AND object_id = OBJECT_ID(N'[dbo].[Logs]'))
                        CREATE NONCLUSTERED INDEX [IX_Logs_Level] ON [dbo].[Logs] ([Level] ASC);
                END");

            // اگر جدول از قبل وجود دارد، مطمئن شو که ستون Message به NVARCHAR(MAX) است
            // این برای اصلاح جداول قدیمی که با NVARCHAR(4000) ایجاد شده‌اند
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
                    BEGIN
                        -- تغییر ستون Message به NVARCHAR(MAX) در صورت نیاز
                        ALTER TABLE [dbo].[Logs] ALTER COLUMN [Message] NVARCHAR(MAX) NULL;
                        
                        -- تغییر ستون MessageTemplate به NVARCHAR(MAX) در صورت نیاز
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND name = 'MessageTemplate')
                            ALTER TABLE [dbo].[Logs] ALTER COLUMN [MessageTemplate] NVARCHAR(MAX) NULL;
                        
                        -- تغییر ستون Level به NVARCHAR(128) در صورت نیاز
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND name = 'Level')
                            ALTER TABLE [dbo].[Logs] ALTER COLUMN [Level] NVARCHAR(128) NULL;
                    END");
                Log.Information("Logs table columns verified/updated to support large messages");
            }
            catch (Exception alterEx)
            {
                Log.Warning(alterEx, "Could not alter Logs table columns. This is normal if columns are already correct.");
            }

            Log.Information("Logs table checked/created successfully");

            // تست نوشتن یک لاگ در دیتابیس
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO [dbo].[Logs] ([Message], [Level], [TimeStamp])
                    VALUES (N'Test log entry - Table creation verified', N'Information', GETUTCDATE())");
                Log.Information("Test log entry written to database successfully");
            }
            catch (Exception testEx)
            {
                Log.Error(testEx, "Failed to write test log entry to database");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not create or verify Logs table. Error details: {ErrorMessage}", ex.Message);
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to apply database migrations");
        throw;
    }
}

// Seed Data
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    await testttt.Infrastructure.Data.DbSeeder.SeedDataAsync(context, userManager);
//}

//// اگر argument --migrate-only وجود داشت، فقط Migration را اجرا می‌کند و برنامه را می‌بندد
//if (args.Contains("--migrate-only"))
//{
//    Log.Information("Migration and seeding completed. Exiting...");
//    return;
//}

// ============================================
// سفارشی Middleware ها - از ساده به پیشرفته
// ============================================
// ترتیب middleware ها بسیار مهم است!
// آنها به ترتیب که اضافه می‌شوند اجرا می‌شوند

// مهم: Authentication و Authorization باید قبل از CORS باشند
app.UseAuthentication();
app.UseAuthorization();

// مهم: Session باید قبل از CORS باشد تا session cookie در response set شود
app.UseSession();

// مهم: CORS باید بعد از Session باشد
app.UseCors("AllowNextJs");

// سطح 6: Rate Limiting (بعد از CORS)
// در Development mode غیرفعال می‌شود
if (!app.Environment.IsDevelopment())
{
    app.UseRateLimiting();
}

// سطح 5: مدیریت خطاهای سراسری (باید زود اضافه شود تا تمام خطاها را catch کند)
app.UseGlobalExceptionHandler();

// سطح 4: لاگ کامل درخواست و پاسخ
app.UseRequestResponseLogging();

// سطح 3: افزودن هدرهای سفارشی
app.UseCustomHeaders();

// سطح 2: اندازه گیری زمان پاسخ
app.UseRequestTiming();

// سطح 1: لاگ ساده (می‌توانید این را غیرفعال کنید اگر از RequestResponseLogging استفاده می‌کنید)
// app.UseSimpleLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// فقط در Production از HTTPS redirect استفاده کنیم (برای اجرای روی HTTP در Development)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Use Serilog Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? Serilog.Events.LogEventLevel.Error
        : elapsed > 500
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information;
});

app.MapControllers();

try
{
    Log.Information("Application started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Application.Services;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;
using testttt.Infrastructure.Repositories;
using testttt.Presentation.Middleware;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

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

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await testttt.Infrastructure.Data.DbSeeder.SeedDataAsync(context, userManager);
}

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

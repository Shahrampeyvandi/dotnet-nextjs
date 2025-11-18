using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Application.Services;
using testttt.Infrastructure.Data;
using testttt.Infrastructure.Repositories;
using testttt.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Entity Framework
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

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
    testttt.Infrastructure.Data.DbSeeder.SeedData(context);
}

// ============================================
// سفارشی Middleware ها - از ساده به پیشرفته
// ============================================
// ترتیب middleware ها بسیار مهم است!
// آنها به ترتیب که اضافه می‌شوند اجرا می‌شوند

// سطح 6: Rate Limiting (باید اول باشد تا درخواست‌های زیاد را فیلتر کند)
app.UseRateLimiting();

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

// Use CORS
app.UseCors("AllowNextJs");

// Use Session
app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using testttt.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace testttt.Infrastructure.Data;

public static class DbSeeder
{
    public static void SeedData(ECommerceDbContext context)
    {
        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "الکترونیک", Description = "لوازم الکترونیکی و دیجیتال", CreatedAt = DateTime.UtcNow },
                new Category { Name = "پوشاک", Description = "لباس و پوشاک", CreatedAt = DateTime.UtcNow },
                new Category { Name = "کتاب", Description = "کتاب و مجلات", CreatedAt = DateTime.UtcNow },
                new Category { Name = "خانه و آشپزخانه", Description = "لوازم خانه و آشپزخانه", CreatedAt = DateTime.UtcNow },
                new Category { Name = "ورزشی", Description = "لوازم ورزشی", CreatedAt = DateTime.UtcNow }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        // Seed Products
        if (!context.Products.Any())
        {
            var categories = context.Categories.ToList();
            var products = new List<Product>
            {
                new Product
                {
                    Name = "گوشی موبایل سامسونگ",
                    Description = "گوشی موبایل سامسونگ گلکسی با صفحه نمایش 6.5 اینچی",
                    Price = 15000000,
                    StockQuantity = 50,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "الکترونیک").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "لپ تاپ لنوو",
                    Description = "لپ تاپ لنوو با پردازنده Intel Core i7",
                    Price = 35000000,
                    StockQuantity = 30,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "الکترونیک").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "تی‌شرت مردانه",
                    Description = "تی‌شرت مردانه پنبه‌ای با کیفیت بالا",
                    Price = 250000,
                    StockQuantity = 100,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "پوشاک").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "کتاب برنامه‌نویسی",
                    Description = "کتاب آموزش برنامه‌نویسی C#",
                    Price = 500000,
                    StockQuantity = 75,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "کتاب").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "ماشین لباسشویی",
                    Description = "ماشین لباسشویی 7 کیلویی",
                    Price = 12000000,
                    StockQuantity = 20,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "خانه و آشپزخانه").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "توپ فوتبال",
                    Description = "توپ فوتبال استاندارد",
                    Price = 300000,
                    StockQuantity = 60,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "ورزشی").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "هدفون بلوتوث",
                    Description = "هدفون بلوتوث با کیفیت صوتی عالی",
                    Price = 2000000,
                    StockQuantity = 40,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "الکترونیک").Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "کفش ورزشی",
                    Description = "کفش ورزشی راحت و با کیفیت",
                    Price = 1500000,
                    StockQuantity = 80,
                    IsActive = true,
                    CategoryId = categories.First(c => c.Name == "ورزشی").Id,
                    CreatedAt = DateTime.UtcNow
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        // Seed Admin User
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = HashPassword("admin123"),
                FirstName = "مدیر",
                LastName = "سیستم",
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}


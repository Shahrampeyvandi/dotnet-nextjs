# راهنمای Migration

## مشکل: "There is already an object named 'Categories' in the database"

این خطا زمانی رخ می‌دهد که جدول‌ها قبلاً در دیتابیس ایجاد شده‌اند (با `EnsureCreatedAsync`) و حالا Migration می‌خواهد دوباره آنها را ایجاد کند.

## راه حل 1: Mark کردن Migration به عنوان Applied (پیشنهادی)

اگر جدول‌ها از قبل در دیتابیس وجود دارند و ساختار آنها با Migration مطابقت دارد:

```powershell
# در Package Manager Console
Update-Database -Project testttt.Infrastructure -StartupProject testttt.Presentation -Migration 0

# سپس Migration را به عنوان applied mark کنید
Update-Database -Project testttt.Infrastructure -StartupProject testttt.Presentation
```

یا در Terminal:
```bash
cd testttt\testttt.Presentation
dotnet ef database update 0 --project ..\testttt.Infrastructure --startup-project .
dotnet ef database update --project ..\testttt.Infrastructure --startup-project .
```

## راه حل 2: حذف جدول‌ها و اجرای Migration

اگر می‌خواهید دیتابیس را از ابتدا با Migration ایجاد کنید:

```sql
-- در SQL Server Management Studio یا dbForge
USE [ECommerceDb]
GO

-- حذف تمام جدول‌ها (مراقب باشید! داده‌ها از دست می‌روند)
DROP TABLE IF EXISTS [dbo].[OrderItems]
DROP TABLE IF EXISTS [dbo].[Orders]
DROP TABLE IF EXISTS [dbo].[Products]
DROP TABLE IF EXISTS [dbo].[Categories]
DROP TABLE IF EXISTS [dbo].[Customers]
DROP TABLE IF EXISTS [dbo].[Logs]

-- حذف جدول‌های Identity
DROP TABLE IF EXISTS [dbo].[AspNetUserTokens]
DROP TABLE IF EXISTS [dbo].[AspNetUserRoles]
DROP TABLE IF EXISTS [dbo].[AspNetUserLogins]
DROP TABLE IF EXISTS [dbo].[AspNetUserClaims]
DROP TABLE IF EXISTS [dbo].[AspNetRoles]
DROP TABLE IF EXISTS [dbo].[AspNetRoleClaims]
DROP TABLE IF EXISTS [dbo].[AspNetUsers]
GO
```

سپس Migration را اجرا کنید:
```powershell
Update-Database -Project testttt.Infrastructure -StartupProject testttt.Presentation
```

## راه حل 3: ایجاد Migration خالی

اگر فقط Identity tables را می‌خواهید اضافه کنید:

```powershell
# حذف Migration فعلی
Remove-Migration -Project testttt.Infrastructure -StartupProject testttt.Presentation

# ایجاد Migration جدید
Add-Migration InitialCreate -Project testttt.Infrastructure -StartupProject testttt.Presentation
```

سپس فایل Migration را ویرایش کنید و فقط Identity tables را نگه دارید.

## بررسی وضعیت Migration

برای بررسی اینکه کدام Migration‌ها اعمال شده‌اند:

```powershell
Get-Migrations -Project testttt.Infrastructure -StartupProject testttt.Presentation
```

یا در Terminal:
```bash
dotnet ef migrations list --project ..\testttt.Infrastructure --startup-project .
```


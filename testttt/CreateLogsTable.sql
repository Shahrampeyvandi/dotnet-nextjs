-- ============================================
-- Script برای ایجاد دستی جدول Logs
-- برای ذخیره لاگ‌های Serilog در دیتابیس
-- ============================================

USE [ECommerceDb]
GO

-- بررسی وجود جدول و حذف آن در صورت وجود (اختیاری)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[Logs]
    PRINT 'جدول Logs قبلاً وجود داشت و حذف شد.'
END
GO

-- ایجاد جدول Logs
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
)
GO

-- ایجاد Index برای TimeStamp (برای جستجوی سریع‌تر بر اساس تاریخ)
CREATE NONCLUSTERED INDEX [IX_Logs_TimeStamp] 
ON [dbo].[Logs] ([TimeStamp] DESC)
GO

-- ایجاد Index برای Level (برای فیلتر کردن بر اساس سطح لاگ)
CREATE NONCLUSTERED INDEX [IX_Logs_Level] 
ON [dbo].[Logs] ([Level] ASC)
GO

-- ایجاد Index ترکیبی برای TimeStamp و Level (برای جستجوهای پیشرفته‌تر)
CREATE NONCLUSTERED INDEX [IX_Logs_TimeStamp_Level] 
ON [dbo].[Logs] ([TimeStamp] DESC, [Level] ASC)
GO

PRINT 'جدول Logs با موفقیت ایجاد شد.'
PRINT 'Indexes برای TimeStamp و Level نیز ایجاد شدند.'
GO

-- ============================================
-- Query های مفید برای بررسی لاگ‌ها
-- ============================================

-- مشاهده تمام لاگ‌ها (آخرین 100 لاگ)
-- SELECT TOP 100 * FROM [dbo].[Logs] ORDER BY [TimeStamp] DESC

-- مشاهده لاگ‌های خطا
-- SELECT * FROM [dbo].[Logs] WHERE [Level] = 'Error' ORDER BY [TimeStamp] DESC

-- مشاهده لاگ‌های امروز
-- SELECT * FROM [dbo].[Logs] 
-- WHERE CAST([TimeStamp] AS DATE) = CAST(GETDATE() AS DATE)
-- ORDER BY [TimeStamp] DESC

-- تعداد لاگ‌ها بر اساس Level
-- SELECT [Level], COUNT(*) AS [Count] 
-- FROM [dbo].[Logs] 
-- GROUP BY [Level] 
-- ORDER BY [Count] DESC

-- حذف لاگ‌های قدیمی‌تر از 30 روز (اختیاری)
-- DELETE FROM [dbo].[Logs] 
-- WHERE [TimeStamp] < DATEADD(DAY, -30, GETDATE())


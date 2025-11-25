-- ============================================
-- Script برای اصلاح ستون Message در جدول Logs
-- تغییر از NVARCHAR(4000) به NVARCHAR(MAX)
-- ============================================

USE [ECommerceDb]
GO

-- بررسی وجود جدول
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND type in (N'U'))
BEGIN
    -- تغییر نوع ستون Message به NVARCHAR(MAX)
    ALTER TABLE [dbo].[Logs]
    ALTER COLUMN [Message] NVARCHAR(MAX) NULL;
    
    PRINT 'ستون Message به NVARCHAR(MAX) تغییر یافت.'
    
    -- همچنین MessageTemplate را هم به MAX تغییر می‌دهیم (در صورت نیاز)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND name = 'MessageTemplate')
    BEGIN
        ALTER TABLE [dbo].[Logs]
        ALTER COLUMN [MessageTemplate] NVARCHAR(MAX) NULL;
        
        PRINT 'ستون MessageTemplate به NVARCHAR(MAX) تغییر یافت.'
    END
    
    -- Level را هم به 128 تغییر می‌دهیم (مطابق با Program.cs)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Logs]') AND name = 'Level')
    BEGIN
        ALTER TABLE [dbo].[Logs]
        ALTER COLUMN [Level] NVARCHAR(128) NULL;
        
        PRINT 'ستون Level به NVARCHAR(128) تغییر یافت.'
    END
END
ELSE
BEGIN
    PRINT 'جدول Logs وجود ندارد. لطفاً ابتدا جدول را ایجاد کنید.'
END
GO

PRINT 'اسکریپت با موفقیت اجرا شد.'
GO


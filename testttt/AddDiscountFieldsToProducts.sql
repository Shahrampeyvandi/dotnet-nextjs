-- Migration: Add Discount Fields to Products Table
-- Date: 2025
-- Description: Adds discount functionality to products including percentage and date ranges

-- Add DiscountPercentage column (decimal 5,2 - allows up to 999.99%)
ALTER TABLE Products
ADD DiscountPercentage DECIMAL(5,2) NULL;

-- Add DiscountStartDate column
ALTER TABLE Products
ADD DiscountStartDate DATETIME2 NULL;

-- Add DiscountEndDate column
ALTER TABLE Products
ADD DiscountEndDate DATETIME2 NULL;

-- Create indexes for better query performance on discount dates
CREATE INDEX IX_Products_DiscountStartDate ON Products(DiscountStartDate);
CREATE INDEX IX_Products_DiscountEndDate ON Products(DiscountEndDate);

-- Add check constraint to ensure discount percentage is between 0 and 100
ALTER TABLE Products
ADD CONSTRAINT CK_Products_DiscountPercentage CHECK (DiscountPercentage IS NULL OR (DiscountPercentage >= 0 AND DiscountPercentage <= 100));

-- Add check constraint to ensure discount end date is after start date (if both are provided)
ALTER TABLE Products
ADD CONSTRAINT CK_Products_DiscountDates CHECK (
    DiscountStartDate IS NULL OR 
    DiscountEndDate IS NULL OR 
    DiscountEndDate >= DiscountStartDate
);


-- Stored Procedure: GetPaginatedOrders
-- Returns flat paginated orders with Customer/User info and item count
-- Parameters: @PageNumber INT, @PageSize INT, @TotalCount INT OUTPUT
-- Returns: Two result sets:
--   1. Flat orders with customer/user info
--   2. Order item counts per order

CREATE OR ALTER PROCEDURE [dbo].[GetPaginatedOrders]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Calculate offset
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Get total count
    SELECT @TotalCount = COUNT(*) FROM [Orders];

    -- First result set: Flat paginated orders with Customer/User data
    SELECT 
        o.[Id],
        o.[OrderNumber],
        o.[OrderDate],
        o.[TotalAmount],
        o.[Status],
        o.[ShippingAddress],
        o.[CreatedAt],
        o.[UpdatedAt],
        o.[CustomerId],
        o.[UserId],
        -- Customer/User name and email (flat)
        COALESCE(
            c.[FirstName] + ' ' + c.[LastName],
            u.[FirstName] + ' ' + u.[LastName],
            NULL
        ) AS CustomerName,
        COALESCE(c.[Email], u.[Email], NULL) AS CustomerEmail,
        COALESCE(c.[Phone], u.[PhoneNumber], NULL) AS CustomerPhone
    FROM [Orders] o
    LEFT JOIN [Customers] c ON o.[CustomerId] = c.[Id]
    LEFT JOIN [AspNetUsers] u ON o.[UserId] = u.[Id]
    ORDER BY o.[OrderDate] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Second result set: Item counts for the orders in the first result set
    SELECT 
        oi.[OrderId],
        COUNT(*) AS ItemsCount
    FROM [OrderItems] oi
    WHERE oi.[OrderId] IN (
        SELECT o.[Id]
        FROM [Orders] o
        ORDER BY o.[OrderDate] DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY
    )
    GROUP BY oi.[OrderId];
END;
GO


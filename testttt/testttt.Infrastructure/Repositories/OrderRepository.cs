using Dapper;
using Microsoft.EntityFrameworkCore;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ECommerceDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<(IEnumerable<OrderListDto> Orders, int TotalCount)> GetPaginatedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var connection = _context.Database.GetDbConnection();
        var wasOpen = connection.State == System.Data.ConnectionState.Open;
        
        if (!wasOpen)
        {
            await connection.OpenAsync();
        }

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            using var multi = await connection.QueryMultipleAsync(
                "[dbo].[GetPaginatedOrders]",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // First result set: Orders with customer/user info
            var ordersData = (await multi.ReadAsync<dynamic>()).ToList();
            
            // Second result set: Item counts per order
            var itemCounts = (await multi.ReadAsync<dynamic>()).ToDictionary(
                x => (int)x.OrderId,
                x => (int)x.ItemsCount
            );

            // Map to OrderListDto and attach item counts
            var orders = ordersData.Select(x => new OrderListDto
            {
                Id = (int)x.Id,
                OrderNumber = (string)x.OrderNumber,
                OrderDate = (DateTime)x.OrderDate,
                TotalAmount = (decimal)x.TotalAmount,
                Status = (string)x.Status,
                ShippingAddress = x.ShippingAddress as string,
                CreatedAt = (DateTime)x.CreatedAt,
                UpdatedAt = x.UpdatedAt as DateTime?,
                CustomerId = x.CustomerId as int?,
                UserId = x.UserId as string,
                CustomerName = x.CustomerName as string,
                CustomerEmail = x.CustomerEmail as string,
                CustomerPhone = x.CustomerPhone as string,
                ItemsCount = itemCounts.TryGetValue((int)x.Id, out var count) ? count : 0
            }).ToList();

            // Get TotalCount from output parameter
            var totalCount = parameters.Get<int>("@TotalCount");

            return (orders, totalCount);
        }
        finally
        {
            if (!wasOpen && connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }
}


using testttt.Domain.Entities;

namespace testttt.Application.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetPaginatedWithDetailsAsync(int pageNumber, int pageSize);
}


using testttt.Application.DTOs;

namespace testttt.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<PaginatedResponse<OrderDto>> GetPaginatedOrdersAsync(int pageNumber, int pageSize);
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
    Task UpdateOrderAsync(int id, UpdateOrderDto updateDto);
    Task DeleteOrderAsync(int id);
    Task<IEnumerable<OrderDto>> GetInvoicesAsync();
}


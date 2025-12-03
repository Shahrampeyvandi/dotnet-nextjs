using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;

namespace testttt.Application.Queries.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order by ID: {OrderId}", request.Id);
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.Id);
        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", request.Id);
            return null;
        }

        var orderDto = order.ToDto();
        _logger.LogInformation("Order {OrderId} retrieved successfully", request.Id);
        return orderDto;
    }
}


using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;

namespace testttt.Application.Queries.Orders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(
        IOrderRepository orderRepository,
        ILogger<GetAllOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all orders");
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        var orderDtos = orders.Select(o => o.ToDto()).ToList();
        _logger.LogInformation("Retrieved {Count} orders successfully", orderDtos.Count);
        return orderDtos;
    }
}


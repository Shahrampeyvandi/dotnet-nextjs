using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;

namespace testttt.Application.Queries.Orders;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetInvoicesQueryHandler> _logger;

    public GetInvoicesQueryHandler(
        IOrderRepository orderRepository,
        ILogger<GetInvoicesQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all invoices");
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        var orderDtos = orders.Select(o => o.ToDto()).ToList();
        _logger.LogInformation("Retrieved {Count} invoices successfully", orderDtos.Count);
        return orderDtos;
    }
}


using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

namespace testttt.Application.Queries.Orders;

public class GetPaginatedOrdersQueryHandler : IRequestHandler<GetPaginatedOrdersQuery, PaginatedResponse<OrderListDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetPaginatedOrdersQueryHandler> _logger;

    public GetPaginatedOrdersQueryHandler(
        IOrderRepository orderRepository,
        ILogger<GetPaginatedOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<PaginatedResponse<OrderListDto>> Handle(GetPaginatedOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting paginated orders. Page: {PageNumber}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (orders, totalCount) = await _orderRepository.GetPaginatedWithDetailsAsync(pageNumber, pageSize);
        var orderList = orders.ToList();

        _logger.LogInformation("Retrieved {Count} orders (page {PageNumber} of {TotalPages})",
            orderList.Count, pageNumber, (int)Math.Ceiling(totalCount / (double)pageSize));

        return new PaginatedResponse<OrderListDto>
        {
            Data = orderList,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}


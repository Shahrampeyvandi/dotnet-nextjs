using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Orders;

public class GetPaginatedOrdersQuery : IRequest<PaginatedResponse<OrderListDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}


using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Orders;

public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
}


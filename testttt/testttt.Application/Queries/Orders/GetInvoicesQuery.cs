using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Orders;

public class GetInvoicesQuery : IRequest<IEnumerable<OrderDto>>
{
}


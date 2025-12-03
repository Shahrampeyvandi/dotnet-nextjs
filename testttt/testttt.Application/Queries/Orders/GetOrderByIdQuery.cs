using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Orders;

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public int Id { get; set; }
}


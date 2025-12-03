using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Commands.Orders;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public int? CustomerId { get; set; }
    public string? UserId { get; set; }
    public string? ShippingAddress { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}


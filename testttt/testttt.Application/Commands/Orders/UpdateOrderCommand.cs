using MediatR;

namespace testttt.Application.Commands.Orders;

public class UpdateOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
}


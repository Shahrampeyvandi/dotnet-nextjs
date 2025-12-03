using MediatR;

namespace testttt.Application.Commands.Orders;

public class DeleteOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
}


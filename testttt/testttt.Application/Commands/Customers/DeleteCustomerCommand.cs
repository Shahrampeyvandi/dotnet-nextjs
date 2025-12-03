using MediatR;

namespace testttt.Application.Commands.Customers;

public class DeleteCustomerCommand : IRequest<Unit>
{
    public int Id { get; set; }
}


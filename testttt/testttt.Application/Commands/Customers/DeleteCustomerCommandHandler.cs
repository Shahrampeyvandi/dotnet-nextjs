using MediatR;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Customers;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found.");
        }

        var hasOrders = await _customerRepository.HasOrdersAsync(request.Id);
        if (hasOrders)
        {
            throw new InvalidOperationException("Cannot delete customer that has orders.");
        }

        await _customerRepository.DeleteAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}


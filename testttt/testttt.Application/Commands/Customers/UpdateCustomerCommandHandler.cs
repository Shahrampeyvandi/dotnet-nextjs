using MediatR;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Customers;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found.");
        }

        var emailExists = await _customerRepository.ExistsAsync(c => c.Email == request.Email && c.Id != request.Id);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.PostalCode = request.PostalCode;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}


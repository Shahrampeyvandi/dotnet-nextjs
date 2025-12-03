using MediatR;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Commands.Customers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _customerRepository.ExistsAsync(c => c.Email == request.Email);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return customer.ToDto();
    }
}


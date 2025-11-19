using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(c => c.ToDto());
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer?.ToDto();
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        var emailExists = await _customerRepository.ExistsAsync(c => c.Email == createDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var customer = new Customer
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            Phone = createDto.Phone,
            Address = createDto.Address,
            City = createDto.City,
            PostalCode = createDto.PostalCode,
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return customer.ToDto();
    }

    public async Task UpdateCustomerAsync(int id, UpdateCustomerDto updateDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        var emailExists = await _customerRepository.ExistsAsync(c => c.Email == updateDto.Email && c.Id != id);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        customer.FirstName = updateDto.FirstName;
        customer.LastName = updateDto.LastName;
        customer.Email = updateDto.Email;
        customer.Phone = updateDto.Phone;
        customer.Address = updateDto.Address;
        customer.City = updateDto.City;
        customer.PostalCode = updateDto.PostalCode;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        var hasOrders = await _customerRepository.HasOrdersAsync(id);
        if (hasOrders)
        {
            throw new InvalidOperationException("Cannot delete customer that has orders.");
        }

        await _customerRepository.DeleteAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
}


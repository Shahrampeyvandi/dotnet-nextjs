using testttt.Application.DTOs;

namespace testttt.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto);
    Task UpdateCustomerAsync(int id, UpdateCustomerDto updateDto);
    Task DeleteCustomerAsync(int id);
}


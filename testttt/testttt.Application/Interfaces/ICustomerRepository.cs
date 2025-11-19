using testttt.Domain.Entities;

namespace testttt.Application.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<bool> HasOrdersAsync(int customerId);
}


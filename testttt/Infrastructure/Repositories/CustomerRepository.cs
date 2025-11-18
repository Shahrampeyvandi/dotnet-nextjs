using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ECommerceDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _dbSet
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<bool> HasOrdersAsync(int customerId)
    {
        return await _context.Orders.AnyAsync(o => o.CustomerId == customerId);
    }
}


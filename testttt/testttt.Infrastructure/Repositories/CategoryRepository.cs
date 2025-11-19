using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ECommerceDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}


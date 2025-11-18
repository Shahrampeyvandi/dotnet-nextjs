using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ECommerceDbContext context) : base(context)
    {
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<bool> HasOrderItemsAsync(int productId)
    {
        return await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);
    }
}


using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;
using testttt.Infrastructure.Extensions;

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
            .OrderByDescending(p => p.CreatedAt)
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

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, int? categoryId = null, bool includeInactive = false)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .AsQueryable();

        // Only filter by IsActive if we don't want to include inactive products
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Apply ordering and pagination using extension method
        var (products, totalCount) = await query
            .ToPaginatedAsync(
                pageNumber,
                pageSize,
                p => p.CreatedAt,
                p => p.Name,
                primaryDescending: true,
                secondaryDescending: false);

        return (products, totalCount);
    }
}


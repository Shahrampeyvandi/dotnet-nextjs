using testttt.Domain.Entities;

namespace testttt.Application.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<bool> HasOrderItemsAsync(int productId);
}


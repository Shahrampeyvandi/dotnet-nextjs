using testttt.Domain.Entities;

namespace testttt.Application.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<bool> HasProductsAsync(int categoryId);
}


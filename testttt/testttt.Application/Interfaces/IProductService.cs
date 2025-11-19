using testttt.Application.DTOs;

namespace testttt.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
    Task UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task DeleteProductAsync(int id);
}


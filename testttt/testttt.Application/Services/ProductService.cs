using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => p.ToDto());
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == createDto.CategoryId);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            StockQuantity = createDto.StockQuantity,
            ImageUrl = createDto.ImageUrl,
            IsActive = createDto.IsActive,
            CategoryId = createDto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == updateDto.CategoryId);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.StockQuantity = updateDto.StockQuantity;
        product.ImageUrl = updateDto.ImageUrl;
        product.IsActive = updateDto.IsActive;
        product.CategoryId = updateDto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        var hasOrderItems = await _productRepository.HasOrderItemsAsync(id);
        if (hasOrderItems)
        {
            throw new InvalidOperationException("Cannot delete product that has been ordered.");
        }

        await _productRepository.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }
}


using MediatR;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Commands.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DiscountPercentage = request.DiscountPercentage,
            DiscountStartDate = request.DiscountStartDate,
            DiscountEndDate = request.DiscountEndDate,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.ToDto();
    }
}


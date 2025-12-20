using MediatR;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
        }

        var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.DiscountPercentage = request.DiscountPercentage;
        product.DiscountStartDate = request.DiscountStartDate;
        product.DiscountEndDate = request.DiscountEndDate;
        product.StockQuantity = request.StockQuantity;
        product.ImageUrl = request.ImageUrl;
        product.IsActive = request.IsActive;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}


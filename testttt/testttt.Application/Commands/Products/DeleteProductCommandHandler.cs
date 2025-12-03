using MediatR;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Products;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
        }

        var hasOrderItems = await _productRepository.HasOrderItemsAsync(request.Id);
        if (hasOrderItems)
        {
            throw new InvalidOperationException("Cannot delete product that has been ordered.");
        }

        await _productRepository.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}


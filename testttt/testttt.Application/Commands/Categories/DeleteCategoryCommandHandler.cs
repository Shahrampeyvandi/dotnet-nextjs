using MediatR;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Categories;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
        }

        var hasProducts = await _categoryRepository.HasProductsAsync(request.Id);
        if (hasProducts)
        {
            throw new InvalidOperationException("Cannot delete category that has products.");
        }

        await _categoryRepository.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}


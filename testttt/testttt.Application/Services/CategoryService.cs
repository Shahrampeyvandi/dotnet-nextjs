using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => c.ToDto());
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category?.ToDto();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
    {
        var category = new Category
        {
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.ToDto();
    }

    public async Task UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        category.Name = updateDto.Name;
        category.Description = updateDto.Description;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        var hasProducts = await _categoryRepository.HasProductsAsync(id);
        if (hasProducts)
        {
            throw new InvalidOperationException("Cannot delete category that has products.");
        }

        await _categoryRepository.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}


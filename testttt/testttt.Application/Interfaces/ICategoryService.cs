using testttt.Application.DTOs;

namespace testttt.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
    Task UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
    Task DeleteCategoryAsync(int id);
}


using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Commands.Categories;

public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}


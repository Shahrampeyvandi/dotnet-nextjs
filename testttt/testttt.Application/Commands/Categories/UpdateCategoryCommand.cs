using MediatR;

namespace testttt.Application.Commands.Categories;

public class UpdateCategoryCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}


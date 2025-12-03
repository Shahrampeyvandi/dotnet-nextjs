using MediatR;

namespace testttt.Application.Commands.Categories;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public int Id { get; set; }
}


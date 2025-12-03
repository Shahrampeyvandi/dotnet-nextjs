using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Categories;

public class GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public int Id { get; set; }
}


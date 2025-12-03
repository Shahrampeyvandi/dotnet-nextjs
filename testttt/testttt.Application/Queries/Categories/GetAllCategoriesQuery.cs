using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Categories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
}


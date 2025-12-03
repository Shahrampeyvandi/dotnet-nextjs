using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Products;

public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}


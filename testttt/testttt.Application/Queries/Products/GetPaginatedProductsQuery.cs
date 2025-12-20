using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Products;

public class GetPaginatedProductsQuery : IRequest<PaginatedResponse<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? CategoryId { get; set; }
    public bool IncludeInactive { get; set; } = false;
}


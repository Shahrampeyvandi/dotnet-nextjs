using MediatR;
using testttt.Application.DTOs;
using testttt.Application.Extensions;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;

namespace testttt.Application.Queries.Products;

public class GetPaginatedProductsQueryHandler : IRequestHandler<GetPaginatedProductsQuery, PaginatedResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetPaginatedProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedResponse<ProductDto>> Handle(GetPaginatedProductsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageSize > 100) request.PageSize = 100; // Limit max page size

        var (products, totalCount) = await _productRepository.GetPaginatedAsync(
            request.PageNumber,
            request.PageSize,
            request.CategoryId,
            request.IncludeInactive
        );

        var productDtos = products.Select(p => p.ToDto()).ToList();

        // Using extension method to create PaginatedResponse
        return productDtos.ToPaginatedResponse(
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}


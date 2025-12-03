using MediatR;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;

namespace testttt.Application.Queries.Products;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        return product?.ToDto();
    }
}


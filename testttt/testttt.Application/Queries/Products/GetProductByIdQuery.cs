using MediatR;
using testttt.Application.DTOs;

namespace testttt.Application.Queries.Products;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public int Id { get; set; }
}


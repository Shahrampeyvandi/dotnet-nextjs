using MediatR;

namespace testttt.Application.Commands.Products;

public class DeleteProductCommand : IRequest<Unit>
{
    public int Id { get; set; }
}


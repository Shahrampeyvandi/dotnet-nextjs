using MediatR;

namespace testttt.Application.Commands.Products;

public class UpdateProductCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
}


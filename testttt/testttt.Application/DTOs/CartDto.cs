namespace testttt.Application.DTOs;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ImageUrl { get; set; }
}

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
}

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}


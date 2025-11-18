namespace testttt.Application.DTOs;

public class OrderItemDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public string? ShippingAddress { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class UpdateOrderDto
{
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
}


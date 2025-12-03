namespace testttt.Application.DTOs;

/// <summary>
/// Flat DTO for displaying orders in list/grid views (no nested collections)
/// </summary>
public class OrderListDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Customer/User info (flat)
    public int? CustomerId { get; set; }
    public string? UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    
    // Summary info (no nested items)
    public int ItemsCount { get; set; }
}


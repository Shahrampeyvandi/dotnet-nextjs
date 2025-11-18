using testttt.Application.DTOs;
using testttt.Domain.Entities;

namespace testttt.Application.Mappings;

public static class MappingProfile
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };
    }

    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            PostalCode = customer.PostalCode,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CustomerId = order.CustomerId,
            CustomerName = order.User != null 
                ? $"{order.User.FirstName} {order.User.LastName}"
                : $"{order.Customer.FirstName} {order.Customer.LastName}",
            CustomerEmail = order.User?.Email ?? order.Customer.Email,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name
            }).ToList()
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Address = user.Address,
            City = user.City,
            PostalCode = user.PostalCode,
            CreatedAt = user.CreatedAt
        };
    }
}


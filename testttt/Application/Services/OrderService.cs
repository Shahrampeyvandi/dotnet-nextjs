using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IUserRepository userRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        return orders.Select(o => o.ToDto());
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        return order?.ToDto();
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        Customer? customer = null;
        if (createDto.CustomerId.HasValue)
        {
            customer = await _customerRepository.GetByIdAsync(createDto.CustomerId.Value);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found.");
            }
        }

        Domain.Entities.User? user = null;
        if (createDto.UserId.HasValue)
        {
            user = await _userRepository.GetByIdAsync(createDto.UserId.Value);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
        }

        if (customer == null && user == null)
        {
            throw new InvalidOperationException("Either CustomerId or UserId must be provided.");
        }

        if (createDto.OrderItems == null || !createDto.OrderItems.Any())
        {
            throw new InvalidOperationException("Order must have at least one item.");
        }

        var orderNumber = GenerateOrderNumber();
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in createDto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException($"Product {product.Name} is not active.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");
            }

            var unitPrice = product.Price;
            var totalPrice = unitPrice * itemDto.Quantity;
            totalAmount += totalPrice;

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            };

            orderItems.Add(orderItem);

            // کاهش موجودی محصول
            product.StockQuantity -= itemDto.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        var order = new Order
        {
            OrderNumber = orderNumber,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = "Pending",
            ShippingAddress = createDto.ShippingAddress,
            CustomerId = customer?.Id ?? 0,
            UserId = user?.Id,
            CreatedAt = DateTime.UtcNow,
            OrderItems = orderItems
        };

        var createdOrder = await _orderRepository.AddAsync(order);
        
        // بارگذاری مجدد برای برگرداندن اطلاعات کامل
        var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(createdOrder.Id);
        return orderWithDetails!.ToDto();
    }

    public async Task UpdateOrderAsync(int id, UpdateOrderDto updateDto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        order.Status = updateDto.Status;
        order.ShippingAddress = updateDto.ShippingAddress;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        // برگرداندن موجودی محصولات
        foreach (var orderItem in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            if (product != null)
            {
                product.StockQuantity += orderItem.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        await _orderRepository.DeleteAsync(order);
    }

    public async Task<IEnumerable<OrderDto>> GetInvoicesAsync()
    {
        return await GetAllOrdersAsync();
    }

    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}


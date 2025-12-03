using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Commands.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        UserManager<ApplicationUser> userManager,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _userManager = userManager;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order. CustomerId: {CustomerId}, UserId: {UserId}, ItemsCount: {ItemsCount}",
            request.CustomerId, request.UserId, request.OrderItems?.Count ?? 0);

        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            Customer? customer = null;
            if (request.CustomerId.HasValue)
            {
                customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                {
                    throw new KeyNotFoundException("Customer not found.");
                }
            }

            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(request.UserId))
            {
                user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
            }

            if (customer == null && user == null)
            {
                throw new InvalidOperationException("Either CustomerId or UserId must be provided.");
            }

            if (request.OrderItems == null || !request.OrderItems.Any())
            {
                throw new InvalidOperationException("Order must have at least one item.");
            }

            var orderNumber = GenerateOrderNumber();
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in request.OrderItems)
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

                product.StockQuantity -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            var order = new Order
            {
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending",
                ShippingAddress = request.ShippingAddress,
                CustomerId = customer?.Id,
                UserId = user?.Id,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Order created successfully. OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                order.Id, orderNumber);

            var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
            return orderWithDetails!.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order. Rolling back transaction");
            await transaction.RollbackAsync();
            throw;
        }
    }

    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}


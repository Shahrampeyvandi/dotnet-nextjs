using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        UserManager<ApplicationUser> userManager,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _userManager = userManager;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        _logger.LogInformation("Getting all orders");
        try
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            var orderDtos = orders.Select(o => o.ToDto()).ToList();
            _logger.LogInformation("Retrieved {Count} orders successfully", orderDtos.Count);
            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all orders");
            throw;
        }
    }

    public async Task<PaginatedResponse<OrderListDto>> GetPaginatedOrdersAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Getting paginated orders. Page: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            var (orders, totalCount) = await _orderRepository.GetPaginatedWithDetailsAsync(pageNumber, pageSize);
            var orderList = orders.ToList();

            _logger.LogInformation("Retrieved {Count} orders (page {PageNumber} of {TotalPages})", 
                orderList.Count, pageNumber, (int)Math.Ceiling(totalCount / (double)pageSize));

            return new PaginatedResponse<OrderListDto>
            {
                Data = orderList,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting paginated orders");
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        _logger.LogInformation("Getting order by ID: {OrderId}", id);
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return null;
            }
            
            var orderDto = order.ToDto();
            _logger.LogInformation("Order {OrderId} retrieved successfully. OrderNumber: {OrderNumber}, TotalAmount: {TotalAmount}", 
                id, orderDto.OrderNumber, orderDto.TotalAmount);
            return orderDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        _logger.LogInformation("Creating new order. CustomerId: {CustomerId}, UserId: {UserId}, ItemsCount: {ItemsCount}", 
            createDto.CustomerId, createDto.UserId, createDto.OrderItems?.Count ?? 0);
        
        // شروع Transaction برای اطمینان از atomic بودن تمام عملیات
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            Customer? customer = null;
            if (createDto.CustomerId.HasValue)
            {
                _logger.LogDebug("Looking up customer with ID: {CustomerId}", createDto.CustomerId.Value);
                customer = await _customerRepository.GetByIdAsync(createDto.CustomerId.Value);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", createDto.CustomerId.Value);
                    throw new KeyNotFoundException("Customer not found.");
                }
                _logger.LogDebug("Customer found: {CustomerName} ({CustomerEmail})", 
                    $"{customer.FirstName} {customer.LastName}", customer.Email);
            }

            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(createDto.UserId))
            {
                _logger.LogDebug("Looking up user with ID: {UserId}", createDto.UserId);
                user = await _userManager.FindByIdAsync(createDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", createDto.UserId);
                    throw new KeyNotFoundException("User not found.");
                }
                _logger.LogDebug("User found: {UserName} ({UserEmail})", 
                    $"{user.FirstName} {user.LastName}", user.Email);
            }

            if (customer == null && user == null)
            {
                _logger.LogError("Neither CustomerId nor UserId provided for order creation");
                throw new InvalidOperationException("Either CustomerId or UserId must be provided.");
            }

            if (createDto.OrderItems == null || !createDto.OrderItems.Any())
            {
                _logger.LogError("Order creation attempted with no items");
                throw new InvalidOperationException("Order must have at least one item.");
            }

            var orderNumber = GenerateOrderNumber();
            _logger.LogInformation("Generated order number: {OrderNumber}", orderNumber);
            
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            // بررسی و کاهش موجودی محصولات
            foreach (var itemDto in createDto.OrderItems)
            {
                _logger.LogDebug("Processing order item: ProductId={ProductId}, Quantity={Quantity}", 
                    itemDto.ProductId, itemDto.Quantity);
                
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    _logger.LogError("Product with ID {ProductId} not found", itemDto.ProductId);
                    throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
                }

                if (!product.IsActive)
                {
                    _logger.LogWarning("Product {ProductName} (ID: {ProductId}) is not active", 
                        product.Name, product.Id);
                    throw new InvalidOperationException($"Product {product.Name} is not active.");
                }

                if (product.StockQuantity < itemDto.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductName} (ID: {ProductId}). Available: {Available}, Requested: {Requested}", 
                        product.Name, product.Id, product.StockQuantity, itemDto.Quantity);
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

                // کاهش موجودی محصول (بدون SaveChanges - در انتها یک بار انجام می‌شود)
                var oldStock = product.StockQuantity;
                product.StockQuantity -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);
                
                _logger.LogDebug("Product {ProductName} (ID: {ProductId}) stock updated: {OldStock} -> {NewStock}", 
                    product.Name, product.Id, oldStock, product.StockQuantity);
            }

            // ایجاد سفارش
            var order = new Order
            {
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending",
                ShippingAddress = createDto.ShippingAddress,
                CustomerId = customer?.Id,
                UserId = user?.Id,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            // اضافه کردن سفارش به context (بدون SaveChanges)
            await _orderRepository.AddAsync(order);
            
            // ذخیره تمام تغییرات در یک transaction (فقط یک بار)
            await _unitOfWork.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            _logger.LogInformation("Order created successfully. OrderId: {OrderId}, OrderNumber: {OrderNumber}, TotalAmount: {TotalAmount}, ItemsCount: {ItemsCount}", 
                order.Id, orderNumber, totalAmount, orderItems.Count);
            
            // بارگذاری مجدد برای برگرداندن اطلاعات کامل
            var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
            return orderWithDetails!.ToDto();
        }
        catch (Exception ex)
        {
            // در صورت خطا، تمام تغییرات را rollback می‌کنیم
            _logger.LogError(ex, "Error occurred while creating order. Rolling back transaction");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateOrderAsync(int id, UpdateOrderDto updateDto)
    {
        _logger.LogInformation("Updating order with ID: {OrderId}. New Status: {Status}", id, updateDto.Status);
        
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for update", id);
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            var oldStatus = order.Status;
            order.Status = updateDto.Status;
            order.ShippingAddress = updateDto.ShippingAddress;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Order {OrderId} updated successfully. OrderNumber: {OrderNumber}, Status changed: {OldStatus} -> {NewStatus}", 
                id, order.OrderNumber, oldStatus, updateDto.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task DeleteOrderAsync(int id)
    {
        _logger.LogInformation("Deleting order with ID: {OrderId}", id);
        
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deletion", id);
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            _logger.LogInformation("Order {OrderId} found. OrderNumber: {OrderNumber}, TotalAmount: {TotalAmount}, ItemsCount: {ItemsCount}", 
                id, order.OrderNumber, order.TotalAmount, order.OrderItems.Count);

            // برگرداندن موجودی محصولات
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                if (product != null)
                {
                    var oldStock = product.StockQuantity;
                    product.StockQuantity += orderItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                    
                    _logger.LogDebug("Product {ProductName} (ID: {ProductId}) stock restored: {OldStock} -> {NewStock} (Quantity: {Quantity})", 
                        product.Name, product.Id, oldStock, product.StockQuantity, orderItem.Quantity);
                }
                else
                {
                    _logger.LogWarning("Product with ID {ProductId} not found while restoring stock for order {OrderId}", 
                        orderItem.ProductId, id);
                }
            }

            await _orderRepository.DeleteAsync(order);
            
            // ذخیره تمام تغییرات (برگرداندن موجودی + حذف سفارش) در یک SaveChanges
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Order {OrderId} (OrderNumber: {OrderNumber}) deleted successfully. Stock quantities restored.", 
                id, order.OrderNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetInvoicesAsync()
    {
        _logger.LogInformation("Getting all invoices");
        try
        {
            var invoices = await GetAllOrdersAsync();
            _logger.LogInformation("Retrieved {Count} invoices successfully", invoices.Count());
            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting invoices");
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


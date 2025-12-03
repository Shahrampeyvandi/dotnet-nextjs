using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Orders;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting order with ID: {OrderId}", request.Id);

        var order = await _orderRepository.GetByIdWithDetailsAsync(request.Id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {request.Id} not found.");
        }

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
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} deleted successfully. Stock quantities restored.", request.Id);
        return Unit.Value;
    }
}


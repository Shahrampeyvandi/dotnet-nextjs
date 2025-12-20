using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.Interfaces;

namespace testttt.Application.Commands.Orders;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order with ID: {OrderId}. New Status: {Status}", request.Id, request.Status);

        var order = await _orderRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {request.Id} not found.");
        }

        order.Status = request.Status;
        order.ShippingAddress = request.ShippingAddress;
        order.UpdatedAt = DateTime.UtcNow;

        if (order.Status == "Cancelled")
        {
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.RealizeQuantity(orderItem.Quantity);
            }
        }

        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} updated successfully", request.Id);
        return Unit.Value;
    }
}


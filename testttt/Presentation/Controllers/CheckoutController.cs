using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using System.Text.Json;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IProductRepository _productRepository;
    private const string CartSessionKey = "Cart";

    public CheckoutController(IOrderService orderService, IProductRepository productRepository)
    {
        _orderService = orderService;
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var cartJson = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrEmpty(cartJson))
        {
            return BadRequest("Cart is empty");
        }

        var cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson);
        if (cartItems == null || !cartItems.Any())
        {
            return BadRequest("Cart is empty");
        }

        var orderItems = cartItems.Select(item => new CreateOrderItemDto
        {
            ProductId = item.Key,
            Quantity = item.Value
        }).ToList();

        var createOrderDto = new CreateOrderDto
        {
            UserId = userId,
            ShippingAddress = checkoutDto.ShippingAddress,
            OrderItems = orderItems
        };

        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            HttpContext.Session.Remove(CartSessionKey);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class CheckoutDto
{
    public string? ShippingAddress { get; set; }
}


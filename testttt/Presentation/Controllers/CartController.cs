using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using System.Text.Json;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private const string CartSessionKey = "Cart";

    public CartController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        var cartItems = string.IsNullOrEmpty(cartJson)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();

        var cartDto = new CartDto();
        decimal totalAmount = 0;
        int totalItems = 0;

        foreach (var item in cartItems)
        {
            var product = await _productRepository.GetByIdAsync(item.Key);
            if (product != null && product.IsActive)
            {
                var itemTotal = product.Price * item.Value;
                totalAmount += itemTotal;
                totalItems += item.Value;

                cartDto.Items.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Value,
                    TotalPrice = itemTotal,
                    ImageUrl = product.ImageUrl
                });
            }
        }

        cartDto.TotalAmount = totalAmount;
        cartDto.TotalItems = totalItems;

        return Ok(cartDto);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDto addToCartDto)
    {
        var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        if (!product.IsActive)
        {
            return BadRequest("Product is not active");
        }

        if (product.StockQuantity < addToCartDto.Quantity)
        {
            return BadRequest("Insufficient stock");
        }

        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        var cartItems = string.IsNullOrEmpty(cartJson)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();

        if (cartItems.ContainsKey(addToCartDto.ProductId))
        {
            cartItems[addToCartDto.ProductId] += addToCartDto.Quantity;
        }
        else
        {
            cartItems[addToCartDto.ProductId] = addToCartDto.Quantity;
        }

        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));
        return Ok(new { message = "Item added to cart" });
    }

    [HttpDelete("{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return NotFound("Cart is empty");
        }

        var cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson);
        if (cartItems == null || !cartItems.ContainsKey(productId))
        {
            return NotFound("Item not found in cart");
        }

        cartItems.Remove(productId);
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));
        return Ok(new { message = "Item removed from cart" });
    }

    [HttpPut("{productId}")]
    public IActionResult UpdateCartItem(int productId, [FromBody] int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("Quantity must be greater than 0");
        }

        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return NotFound("Cart is empty");
        }

        var cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson);
        if (cartItems == null || !cartItems.ContainsKey(productId))
        {
            return NotFound("Item not found in cart");
        }

        cartItems[productId] = quantity;
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));
        return Ok(new { message = "Cart updated" });
    }

    [HttpPost("clear")]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return Ok(new { message = "Cart cleared" });
    }
}


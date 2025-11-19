using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using System.Text.Json;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string CartSessionKey = "Cart";

    public CheckoutController(
        IOrderService orderService, 
        IProductRepository productRepository,
        UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _productRepository = productRepository;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OrderDto>> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        // Load session
        await HttpContext.Session.LoadAsync();

        // دریافت کاربر از Identity
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id;
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

        // بررسی موجودی محصولات قبل از ایجاد سفارش
        var validationErrors = new List<string>();
        var updatedCartItems = new Dictionary<int, int>();

        foreach (var item in cartItems)
        {
            var product = await _productRepository.GetByIdAsync(item.Key);
            if (product == null)
            {
                validationErrors.Add($"محصول با شناسه {item.Key} یافت نشد.");
                continue;
            }

            if (!product.IsActive)
            {
                validationErrors.Add($"محصول {product.Name} غیرفعال است.");
                continue;
            }

            if (product.StockQuantity < item.Value)
            {
                // اگر موجودی کافی نیست، مقدار را به موجودی موجود محدود می‌کنیم
                if (product.StockQuantity > 0)
                {
                    updatedCartItems[item.Key] = product.StockQuantity;
                    validationErrors.Add($"موجودی محصول {product.Name} کافی نیست. مقدار به {product.StockQuantity} عدد کاهش یافت.");
                }
                else
                {
                    validationErrors.Add($"محصول {product.Name} موجود نیست.");
                }
            }
            else
            {
                updatedCartItems[item.Key] = item.Value;
            }
        }

        // اگر همه محصولات حذف شدند
        if (!updatedCartItems.Any())
        {
            HttpContext.Session.Remove(CartSessionKey);
            await HttpContext.Session.CommitAsync();
            return BadRequest(new { 
                message = "هیچ محصولی در سبد خرید موجود نیست.",
                errors = validationErrors 
            });
        }

        // اگر cart به‌روزرسانی شد، آن را ذخیره می‌کنیم
        if (updatedCartItems.Count != cartItems.Count || 
            updatedCartItems.Any(x => !cartItems.ContainsKey(x.Key) || cartItems[x.Key] != x.Value))
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(updatedCartItems));
            await HttpContext.Session.CommitAsync();
            
            return BadRequest(new { 
                message = "موجودی برخی محصولات تغییر کرده است. لطفا سبد خرید را بررسی کنید.",
                errors = validationErrors,
                cartUpdated = true
            });
        }

        var orderItems = updatedCartItems.Select(item => new CreateOrderItemDto
        {
            ProductId = item.Key,
            Quantity = item.Value
        }).ToList();

        var createOrderDto = new CreateOrderDto
        {
            UserId = userId, // string for Identity
            ShippingAddress = checkoutDto.ShippingAddress,
            OrderItems = orderItems
        };

        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            HttpContext.Session.Remove(CartSessionKey);
            await HttpContext.Session.CommitAsync();
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            // خطاهای مربوط به موجودی یا وضعیت محصول
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CheckoutDto
{
    public string? ShippingAddress { get; set; }
}


using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

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
        // مهم: Load session به صورت دستی برای اطمینان از initialize شدن
        await HttpContext.Session.LoadAsync();

        // Debug: بررسی session
        var sessionId = HttpContext.Session.Id;
        var isSessionAvailable = HttpContext.Session.IsAvailable;

        // بررسی cookies در request
        var requestCookies = HttpContext.Request.Cookies;
        var sessionCookie = requestCookies[".AspNetCore.Session"];

        // Debug: بررسی تمام cookies در request
        var allCookies = string.Join(", ", requestCookies.Keys);
        System.Diagnostics.Debug.WriteLine($"[CartController] GetCart - Session ID: {sessionId}, Available: {isSessionAvailable}, Cookie: {(sessionCookie != null ? $"exists ({sessionCookie.Substring(0, Math.Min(20, sessionCookie.Length))}...)" : "null")}, All Cookies: [{allCookies}]");

        // اگر session موجود نباشد، یک session جدید ایجاد می‌شود
        // اما فقط اگر cookie در request موجود نباشد
        if (!isSessionAvailable && sessionCookie == null)
        {
            System.Diagnostics.Debug.WriteLine($"[CartController] GetCart - Creating new session because no cookie found");
            HttpContext.Session.SetString("_Initialized", DateTime.UtcNow.ToString());
            await HttpContext.Session.CommitAsync();
        }

        var cartJson = HttpContext.Session.GetString(CartSessionKey);

        // Log برای debug
        System.Diagnostics.Debug.WriteLine($"[CartController] GetCart - Cart: {(string.IsNullOrEmpty(cartJson) ? "null" : "exists")}, Session ID: {sessionId}, Cookie Value: {(sessionCookie != null ? sessionCookie.Substring(0, Math.Min(20, sessionCookie.Length)) : "null")}");

        var cartItems = string.IsNullOrEmpty(cartJson)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();

        // Commit session برای اطمینان از set شدن cookie (حتی اگر تغییر نکرده باشد)
        await HttpContext.Session.CommitAsync();

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
        // مهم: Load session به صورت دستی - این session را initialize می‌کند
        await HttpContext.Session.LoadAsync();

        // اگر session موجود نباشد، یک session جدید ایجاد می‌شود
        if (!HttpContext.Session.IsAvailable)
        {
            // Force session creation by setting a value
            HttpContext.Session.SetString("_Initialized", "true");
        }

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

        // Debug: بررسی cart قبل از تغییر
        System.Diagnostics.Debug.WriteLine($"[CartController] AddToCart - Before: CartJson is {(string.IsNullOrEmpty(cartJson) ? "null" : "exists")}");

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

        // Debug: بررسی cart بعد از تغییر
        var newCartJson = JsonSerializer.Serialize(cartItems);
        System.Diagnostics.Debug.WriteLine($"[CartController] AddToCart - After: CartItems count: {cartItems.Count}, CartJson length: {newCartJson.Length}");

        HttpContext.Session.SetString(CartSessionKey, newCartJson);

        // Debug: بررسی session قبل از commit
        var beforeCommit = HttpContext.Session.GetString(CartSessionKey);
        System.Diagnostics.Debug.WriteLine($"[CartController] AddToCart - Before Commit: CartJson is {(string.IsNullOrEmpty(beforeCommit) ? "null" : "exists")}");

        // Commit session changes - این session cookie را در response set می‌کند
        await HttpContext.Session.CommitAsync();

        // Debug: بررسی session بعد از commit
        await HttpContext.Session.LoadAsync(); // Reload برای بررسی
        var afterCommit = HttpContext.Session.GetString(CartSessionKey);
        var finalSessionId = HttpContext.Session.Id;

        // بررسی cookies در response
        var responseCookies = HttpContext.Response.Cookies;

        System.Diagnostics.Debug.WriteLine($"[CartController] AddToCart - After Commit: CartJson is {(string.IsNullOrEmpty(afterCommit) ? "null" : "exists")}, Session ID: {finalSessionId}, Cookie in Response: {(responseCookies != null ? "exists" : "null")}");

        return Ok(new { message = "Item added to cart", sessionId = finalSessionId });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        await HttpContext.Session.LoadAsync();

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
        await HttpContext.Session.CommitAsync();

        return Ok(new { message = "Item removed from cart" });
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateCartItem(int productId, [FromBody] int quantity)
    {
        await HttpContext.Session.LoadAsync();

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
        await HttpContext.Session.CommitAsync();

        return Ok(new { message = "Cart updated" });
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await HttpContext.Session.LoadAsync();
        HttpContext.Session.Remove(CartSessionKey);
        await HttpContext.Session.CommitAsync();

        return Ok(new { message = "Cart cleared" });
    }
}


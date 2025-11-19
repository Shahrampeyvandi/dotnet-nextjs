using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createDto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(createDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Orders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateDto)
    {
        try
        {
            await _orderService.UpdateOrderAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET: api/Orders/invoices
    [HttpGet("invoices")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetInvoices()
    {
        var invoices = await _orderService.GetInvoicesAsync();
        return Ok(invoices);
    }
}


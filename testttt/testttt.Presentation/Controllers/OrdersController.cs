using MediatR;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.Commands.Orders;
using testttt.Application.DTOs;
using testttt.Application.Queries.Orders;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        // If pagination parameters are provided, return paginated response
        if (page.HasValue && pageSize.HasValue)
        {
            var query = new GetPaginatedOrdersQuery
            {
                PageNumber = page.Value,
                PageSize = pageSize.Value
            };
            var paginatedOrders = await _mediator.Send(query);
            return Ok(paginatedOrders);
        }
        
        // Otherwise, return all orders (backward compatibility)
        var getAllQuery = new GetAllOrdersQuery();
        var orders = await _mediator.Send(getAllQuery);
        return Ok(orders);
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var query = new GetOrderByIdQuery { Id = id };
        var order = await _mediator.Send(query);

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
            var command = new CreateOrderCommand
            {
                CustomerId = createDto.CustomerId,
                UserId = createDto.UserId,
                ShippingAddress = createDto.ShippingAddress,
                OrderItems = createDto.OrderItems
            };
            var order = await _mediator.Send(command);
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
            var command = new UpdateOrderCommand
            {
                Id = id,
                Status = updateDto.Status,
                ShippingAddress = updateDto.ShippingAddress
            };
            await _mediator.Send(command);
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
            var command = new DeleteOrderCommand { Id = id };
            await _mediator.Send(command);
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
        var query = new GetInvoicesQuery();
        var invoices = await _mediator.Send(query);
        return Ok(invoices);
    }
}


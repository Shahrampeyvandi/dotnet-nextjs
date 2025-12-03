using MediatR;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.Commands.Customers;
using testttt.Application.DTOs;
using testttt.Application.Queries.Customers;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var query = new GetAllCustomersQuery();
        var customers = await _mediator.Send(query);
        return Ok(customers);
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var query = new GetCustomerByIdQuery { Id = id };
        var customer = await _mediator.Send(query);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    // POST: api/Customers
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createDto)
    {
        try
        {
            var command = new CreateCustomerCommand
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Address = createDto.Address,
                City = createDto.City,
                PostalCode = createDto.PostalCode
            };
            var customer = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateDto)
    {
        try
        {
            var command = new UpdateCustomerCommand
            {
                Id = id,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                Email = updateDto.Email,
                Phone = updateDto.Phone,
                Address = updateDto.Address,
                City = updateDto.City,
                PostalCode = updateDto.PostalCode
            };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var command = new DeleteCustomerCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


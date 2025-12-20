using MediatR;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.Commands.Products;
using testttt.Application.DTOs;
using testttt.Application.Queries.Products;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // GET: api/Products/paginated?pageNumber=1&pageSize=10&categoryId=1&includeInactive=false
    [HttpGet("paginated")]
    public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetPaginatedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool includeInactive = false)
    {
        var query = new GetPaginatedProductsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            CategoryId = categoryId,
            IncludeInactive = includeInactive
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var product = await _mediator.Send(query);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto)
    {
        try
        {
            var command = new CreateProductCommand
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                DiscountPercentage = createDto.DiscountPercentage,
                DiscountStartDate = createDto.DiscountStartDate,
                DiscountEndDate = createDto.DiscountEndDate,
                StockQuantity = createDto.StockQuantity,
                ImageUrl = createDto.ImageUrl,
                IsActive = createDto.IsActive,
                CategoryId = createDto.CategoryId
            };
            var product = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateDto)
    {
        try
        {
            var command = new UpdateProductCommand
            {
                Id = id,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Price = updateDto.Price,
                DiscountPercentage = updateDto.DiscountPercentage,
                DiscountStartDate = updateDto.DiscountStartDate,
                DiscountEndDate = updateDto.DiscountEndDate,
                StockQuantity = updateDto.StockQuantity,
                ImageUrl = updateDto.ImageUrl,
                IsActive = updateDto.IsActive,
                CategoryId = updateDto.CategoryId
            };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var command = new DeleteProductCommand { Id = id };
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


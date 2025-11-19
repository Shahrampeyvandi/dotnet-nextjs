using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

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
            var product = await _productService.CreateProductAsync(createDto);
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
            await _productService.UpdateProductAsync(id, updateDto);
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
            await _productService.DeleteProductAsync(id);
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


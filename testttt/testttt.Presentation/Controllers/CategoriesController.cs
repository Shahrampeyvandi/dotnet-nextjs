using MediatR;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.Commands.Categories;
using testttt.Application.DTOs;
using testttt.Application.Queries.Categories;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query);
        return Ok(categories);
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var category = await _mediator.Send(query);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
    {
        try
        {
            var command = new CreateCategoryCommand
            {
                Name = createDto.Name,
                Description = createDto.Description
            };
            var category = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateDto)
    {
        try
        {
            var command = new UpdateCategoryCommand
            {
                Id = id,
                Name = updateDto.Name,
                Description = updateDto.Description
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

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var command = new DeleteCategoryCommand { Id = id };
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


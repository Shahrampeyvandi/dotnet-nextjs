using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser(UpdateUserDto updateDto)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            await _userService.UpdateUserAsync(userId.Value, updateDto);
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

    [HttpGet("me/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized();
        }

        var orders = await _userService.GetUserOrdersAsync(userId.Value);
        return Ok(orders);
    }
}


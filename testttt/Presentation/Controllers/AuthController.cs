using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registerDto);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userService.LoginAsync(loginDto);
        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);
        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logged out successfully" });
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
}


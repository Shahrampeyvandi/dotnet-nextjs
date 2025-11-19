using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        try
        {
            // بررسی اینکه آیا کاربر با این username یا email وجود دارد
            var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUserByUsername != null)
            {
                return BadRequest("Username already exists.");
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUserByEmail != null)
            {
                return BadRequest("Email already exists.");
            }

            // ایجاد کاربر جدید
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            // Sign in کاربر بعد از ثبت‌نام
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // پیدا کردن کاربر با username یا email
        var user = await _userManager.FindByNameAsync(loginDto.Username) 
                   ?? await _userManager.FindByEmailAsync(loginDto.Username);
        
        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        // بررسی password
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            loginDto.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return Unauthorized("Account is locked out. Please try again later.");
            }
            return Unauthorized("Invalid username or password");
        }

        return Ok(user.ToDto());
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user.ToDto());
    }
}


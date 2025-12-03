using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOtpService _otpService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOtpService otpService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _otpService = otpService;
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

            var userDto = user.ToDto();
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Request OTP code for registration (event-driven - sends SMS via Kavehnegar)
    /// </summary>
    [HttpPost("request-otp")]
    public async Task<ActionResult<RequestOtpResponseDto>> RequestOtp([FromBody] RequestOtpDto requestOtpDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestOtpDto.PhoneNumber))
            {
                return BadRequest(new RequestOtpResponseDto
                {
                    Success = false,
                    Message = "Phone number is required"
                });
            }

            // Generate and send OTP (event-driven - will trigger SMS sending)
            await _otpService.GenerateOtpAsync(
                requestOtpDto.PhoneNumber, 
                requestOtpDto.Purpose, 
                expirationMinutes: 5);

            return Ok(new RequestOtpResponseDto
            {
                Success = true,
                Message = "OTP code has been sent to your phone number",
                ExpirationMinutes = 5
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new RequestOtpResponseDto
            {
                Success = false,
                Message = $"Error requesting OTP: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Verify OTP code
    /// </summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<VerifyOtpResponseDto>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(verifyOtpDto.PhoneNumber) || string.IsNullOrWhiteSpace(verifyOtpDto.Code))
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Phone number and OTP code are required"
                });
            }

            var isValid = await _otpService.VerifyOtpAsync(
                verifyOtpDto.PhoneNumber, 
                verifyOtpDto.Code, 
                verifyOtpDto.Purpose);

            if (!isValid)
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired OTP code"
                });
            }

            return Ok(new VerifyOtpResponseDto
            {
                Success = true,
                Message = "OTP verified successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new VerifyOtpResponseDto
            {
                Success = false,
                Message = $"Error verifying OTP: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Register with OTP verification (event-driven registration)
    /// </summary>
    [HttpPost("register-with-otp")]
    public async Task<ActionResult<UserDto>> RegisterWithOtp([FromBody] RegisterWithOtpDto registerDto)
    {
        try
        {
            // Verify OTP first
            var isOtpValid = await _otpService.VerifyOtpAsync(
                registerDto.PhoneNumber, 
                registerDto.OtpCode, 
                "Registration");

            if (!isOtpValid)
            {
                return BadRequest("Invalid or expired OTP code. Please request a new OTP.");
            }

            // Check if user already exists
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

            // Check if phone number is already registered
            var existingUserByPhone = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingUserByPhone != null)
            {
                return BadRequest("Phone number is already registered.");
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedAt = DateTime.UtcNow,
                PhoneNumberConfirmed = true // Mark as confirmed since OTP was verified
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            // Sign in user after registration
            await _signInManager.SignInAsync(user, isPersistent: false);

            var userDto = user.ToDto();
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
            return Ok(userDto);
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

        var userDto = user.ToDto();
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();
        return Ok(userDto);
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

        var userDto = user.ToDto();
        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return Ok(userDto);
    }
}


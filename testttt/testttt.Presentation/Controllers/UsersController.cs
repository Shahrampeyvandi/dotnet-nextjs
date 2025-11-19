using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrderRepository _orderRepository;

    public UsersController(UserManager<ApplicationUser> userManager, IOrderRepository orderRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
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

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser(UpdateUserDto updateDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            // بررسی اینکه آیا email تکراری است
            if (user.Email != updateDto.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(updateDto.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest("Email already exists.");
                }
            }

            // به‌روزرسانی اطلاعات کاربر
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.Phone;
            user.Address = updateDto.Address;
            user.City = updateDto.City;
            user.PostalCode = updateDto.PostalCode;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("me/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var orders = await _orderRepository.FindAsync(o => o.UserId == user.Id);
        return Ok(orders.Select(o => o.ToDto()));
    }
}

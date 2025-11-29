using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testttt.Application.DTOs;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var userDto = user.ToDto();
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
            userDtos.Add(userDto);
        }

        return Ok(userDtos);
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Add a role to a user
    /// </summary>
    [HttpPost("users/{userId}/roles/{roleName}")]
    public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Check if role exists
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            // Create role if it doesn't exist
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                return BadRequest($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Check if user already has this role
        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains(roleName))
        {
            return BadRequest("User already has this role");
        }

        var addResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addResult.Succeeded)
        {
            return BadRequest($"Failed to add role: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
        }

        return Ok(new { message = $"Role '{roleName}' added to user successfully" });
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    [HttpDelete("users/{userId}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Check if user has this role
        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(roleName))
        {
            return BadRequest("User does not have this role");
        }

        var removeResult = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!removeResult.Succeeded)
        {
            return BadRequest($"Failed to remove role: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
        }

        return Ok(new { message = $"Role '{roleName}' removed from user successfully" });
    }
}


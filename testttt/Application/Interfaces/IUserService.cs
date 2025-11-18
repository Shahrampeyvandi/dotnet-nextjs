using testttt.Application.DTOs;

namespace testttt.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(int id, UpdateUserDto updateDto);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
}


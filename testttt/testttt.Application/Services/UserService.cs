using System.Security.Cryptography;
using System.Text;
using testttt.Application.DTOs;
using testttt.Application.Interfaces;
using testttt.Application.Mappings;
using testttt.Domain.Entities;

namespace testttt.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        var usernameExists = await _userRepository.UsernameExistsAsync(registerDto.Username);
        if (usernameExists)
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null)
        {
            return null;
        }

        var passwordHash = HashPassword(loginDto.Password);
        if (user.PasswordHash != passwordHash)
        {
            return null;
        }

        return user.ToDto();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.ToDto();
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        var emailExists = await _userRepository.EmailExistsAsync(updateDto.Email);
        if (emailExists && user.Email != updateDto.Email)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.Email = updateDto.Email;
        user.Phone = updateDto.Phone;
        user.Address = updateDto.Address;
        user.City = updateDto.City;
        user.PostalCode = updateDto.PostalCode;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
        return orders.Select(o => o.ToDto());
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}


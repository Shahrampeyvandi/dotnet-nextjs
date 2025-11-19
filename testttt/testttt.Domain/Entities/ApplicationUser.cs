using Microsoft.AspNetCore.Identity;

namespace testttt.Domain.Entities;

/// <summary>
/// Application User که از IdentityUser ارث‌بری می‌کند
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}


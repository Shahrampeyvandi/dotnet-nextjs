namespace testttt.Domain.Entities;

/// <summary>
/// Entity for storing OTP codes for phone number verification
/// </summary>
public class OtpCode
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public string? Purpose { get; set; } // "Registration", "Login", etc.
    public int Attempts { get; set; } = 0; // Number of verification attempts
}


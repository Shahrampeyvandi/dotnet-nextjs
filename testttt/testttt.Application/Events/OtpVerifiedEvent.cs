using MediatR;

namespace testttt.Application.Events;

/// <summary>
/// Event raised when an OTP code is successfully verified
/// </summary>
public class OtpVerifiedEvent : INotification
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}


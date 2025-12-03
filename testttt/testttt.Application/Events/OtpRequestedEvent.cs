using MediatR;

namespace testttt.Application.Events;

/// <summary>
/// Event raised when an OTP code is requested
/// </summary>
public class OtpRequestedEvent : INotification
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}


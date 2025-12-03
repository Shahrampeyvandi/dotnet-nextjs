using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.Events;

namespace testttt.Application.Handlers;

/// <summary>
/// Event handler for OTP verified event - can be used for logging, analytics, etc.
/// </summary>
public class OtpVerifiedEventHandler : INotificationHandler<OtpVerifiedEvent>
{
    private readonly ILogger<OtpVerifiedEventHandler> _logger;

    public OtpVerifiedEventHandler(ILogger<OtpVerifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OtpVerifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OTP verified for phone number {PhoneNumber}, purpose: {Purpose} at {VerifiedAt}", 
            notification.PhoneNumber, notification.Purpose, notification.VerifiedAt);
        
        // Here you can add additional logic like:
        // - Analytics tracking
        // - Notification to other services
        // - Audit logging
        // etc.
        
        return Task.CompletedTask;
    }
}


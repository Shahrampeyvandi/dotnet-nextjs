using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.Events;
using testttt.Application.Interfaces;

namespace testttt.Application.Handlers;

/// <summary>
/// Event handler for OTP requested event - sends SMS via Kavehnegar
/// </summary>
public class OtpRequestedEventHandler : INotificationHandler<OtpRequestedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<OtpRequestedEventHandler> _logger;

    public OtpRequestedEventHandler(
        ISmsService smsService,
        ILogger<OtpRequestedEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(OtpRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling OTP requested event for phone number {PhoneNumber}, purpose: {Purpose}", 
                notification.PhoneNumber, notification.Purpose);

            var success = await _smsService.SendOtpAsync(notification.PhoneNumber, notification.OtpCode);
            
            if (success)
            {
                _logger.LogInformation("OTP SMS sent successfully to {PhoneNumber}", notification.PhoneNumber);
            }
            else
            {
                _logger.LogError("Failed to send OTP SMS to {PhoneNumber}", notification.PhoneNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OTP requested event for phone number {PhoneNumber}", 
                notification.PhoneNumber);
        }
    }
}


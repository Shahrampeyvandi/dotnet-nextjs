using MediatR;
using Microsoft.Extensions.Logging;
using testttt.Application.Events;
using testttt.Application.Interfaces;

namespace testttt.Application.Services;

/// <summary>
/// Service for managing OTP codes
/// </summary>
public class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<OtpService> _logger;
    private readonly Random _random = new();

    public OtpService(
        IOtpRepository otpRepository,
        IMediator mediator,
        ILogger<OtpService> logger)
    {
        _otpRepository = otpRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<string> GenerateOtpAsync(string phoneNumber, string purpose, int expirationMinutes = 5)
    {
        // Generate 6-digit OTP code
        var otpCode = _random.Next(100000, 999999).ToString();
        
        // Check if there's an existing active OTP for this phone number
        var existingOtp = await _otpRepository.GetActiveOtpByPhoneNumberAsync(phoneNumber, purpose);
        
        if (existingOtp != null)
        {
            // Update existing OTP
            existingOtp.Code = otpCode;
            existingOtp.CreatedAt = DateTime.UtcNow;
            existingOtp.ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            existingOtp.IsUsed = false;
            existingOtp.UsedAt = null;
            existingOtp.Attempts = 0;
            await _otpRepository.UpdateOtpAsync(existingOtp);
        }
        else
        {
            // Create new OTP
            var otp = new Domain.Entities.OtpCode
            {
                PhoneNumber = phoneNumber,
                Code = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Purpose = purpose,
                IsUsed = false,
                Attempts = 0
            };
            
            await _otpRepository.CreateOtpAsync(otp);
        }

        // Publish event for sending SMS (event-driven)
        var otpRequestedEvent = new OtpRequestedEvent
        {
            PhoneNumber = phoneNumber,
            OtpCode = otpCode,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        await _mediator.Publish(otpRequestedEvent);
        
        _logger.LogInformation("OTP generated for phone number {PhoneNumber}, purpose: {Purpose}", phoneNumber, purpose);
        
        return otpCode;
    }

    public async Task<bool> VerifyOtpAsync(string phoneNumber, string code, string purpose)
    {
        var otp = await _otpRepository.GetOtpByPhoneNumberAndCodeAsync(phoneNumber, code, purpose);
        
        if (otp == null)
        {
            _logger.LogWarning("OTP not found for phone number {PhoneNumber}, code: {Code}", phoneNumber, code);
            return false;
        }

        // Increment attempts
        otp.Attempts++;
        
        // Check if OTP is expired
        if (otp.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("OTP expired for phone number {PhoneNumber}", phoneNumber);
            await _otpRepository.UpdateOtpAsync(otp);
            return false;
        }

        // Check if OTP is already used
        if (otp.IsUsed)
        {
            _logger.LogWarning("OTP already used for phone number {PhoneNumber}", phoneNumber);
            await _otpRepository.UpdateOtpAsync(otp);
            return false;
        }

        // Mark as used
        otp.IsUsed = true;
        otp.UsedAt = DateTime.UtcNow;
        await _otpRepository.UpdateOtpAsync(otp);

        // Publish verification event
        var otpVerifiedEvent = new OtpVerifiedEvent
        {
            PhoneNumber = phoneNumber,
            Purpose = purpose,
            VerifiedAt = DateTime.UtcNow
        };

        await _mediator.Publish(otpVerifiedEvent);
        
        _logger.LogInformation("OTP verified successfully for phone number {PhoneNumber}, purpose: {Purpose}", phoneNumber, purpose);
        
        return true;
    }

    public async Task<bool> IsOtpValidAsync(string phoneNumber, string code, string purpose)
    {
        var otp = await _otpRepository.GetOtpByPhoneNumberAndCodeAsync(phoneNumber, code, purpose);
        
        if (otp == null)
        {
            return false;
        }

        if (otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    public async Task MarkOtpAsUsedAsync(string phoneNumber, string code, string purpose)
    {
        var otp = await _otpRepository.GetOtpByPhoneNumberAndCodeAsync(phoneNumber, code, purpose);
        
        if (otp != null)
        {
            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            await _otpRepository.UpdateOtpAsync(otp);
        }
    }
}


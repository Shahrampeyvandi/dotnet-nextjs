namespace testttt.Application.Interfaces;

/// <summary>
/// Interface for SMS service providers
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS message
    /// </summary>
    /// <param name="phoneNumber">Recipient phone number</param>
    /// <param name="message">Message content</param>
    /// <returns>True if sent successfully, false otherwise</returns>
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    
    /// <summary>
    /// Sends an OTP code via SMS
    /// </summary>
    /// <param name="phoneNumber">Recipient phone number</param>
    /// <param name="otpCode">OTP code to send</param>
    /// <returns>True if sent successfully, false otherwise</returns>
    Task<bool> SendOtpAsync(string phoneNumber, string otpCode);
}


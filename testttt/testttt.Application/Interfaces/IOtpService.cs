namespace testttt.Application.Interfaces;

/// <summary>
/// Interface for OTP code management
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates and stores a new OTP code for a phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to generate OTP for</param>
    /// <param name="purpose">Purpose of the OTP (e.g., "Registration")</param>
    /// <param name="expirationMinutes">Expiration time in minutes (default: 5)</param>
    /// <returns>The generated OTP code</returns>
    Task<string> GenerateOtpAsync(string phoneNumber, string purpose, int expirationMinutes = 5);
    
    /// <summary>
    /// Verifies an OTP code for a phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="code">OTP code to verify</param>
    /// <param name="purpose">Purpose of the OTP</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> VerifyOtpAsync(string phoneNumber, string code, string purpose);
    
    /// <summary>
    /// Checks if an OTP code exists and is valid (not expired, not used)
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="code">OTP code to check</param>
    /// <param name="purpose">Purpose of the OTP</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> IsOtpValidAsync(string phoneNumber, string code, string purpose);
    
    /// <summary>
    /// Marks an OTP code as used
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="code">OTP code</param>
    /// <param name="purpose">Purpose of the OTP</param>
    Task MarkOtpAsUsedAsync(string phoneNumber, string code, string purpose);
}


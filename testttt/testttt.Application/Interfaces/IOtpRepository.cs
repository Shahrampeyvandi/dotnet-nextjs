using testttt.Domain.Entities;

namespace testttt.Application.Interfaces;

public interface IOtpRepository
{
    Task<OtpCode?> GetActiveOtpByPhoneNumberAsync(string phoneNumber, string purpose);
    Task<OtpCode?> GetOtpByPhoneNumberAndCodeAsync(string phoneNumber, string code, string purpose);
    Task<OtpCode> CreateOtpAsync(OtpCode otpCode);
    Task UpdateOtpAsync(OtpCode otpCode);
    Task DeleteExpiredOtpsAsync();
}


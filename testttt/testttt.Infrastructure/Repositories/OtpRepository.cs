using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Domain.Entities;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ECommerceDbContext _context;

    public OtpRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<OtpCode?> GetActiveOtpByPhoneNumberAsync(string phoneNumber, string purpose)
    {
        return await _context.Set<OtpCode>()
            .Where(o => o.PhoneNumber == phoneNumber 
                     && o.Purpose == purpose 
                     && !o.IsUsed 
                     && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<OtpCode?> GetOtpByPhoneNumberAndCodeAsync(string phoneNumber, string code, string purpose)
    {
        return await _context.Set<OtpCode>()
            .FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber 
                                   && o.Code == code 
                                   && o.Purpose == purpose);
    }

    public async Task<OtpCode> CreateOtpAsync(OtpCode otpCode)
    {
        _context.Set<OtpCode>().Add(otpCode);
        await _context.SaveChangesAsync();
        return otpCode;
    }

    public async Task UpdateOtpAsync(OtpCode otpCode)
    {
        _context.Set<OtpCode>().Update(otpCode);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredOtpsAsync()
    {
        var expiredOtps = await _context.Set<OtpCode>()
            .Where(o => o.ExpiresAt < DateTime.UtcNow.AddDays(-1)) // Delete OTPs expired more than 1 day ago
            .ToListAsync();
        
        _context.Set<OtpCode>().RemoveRange(expiredOtps);
        await _context.SaveChangesAsync();
    }
}


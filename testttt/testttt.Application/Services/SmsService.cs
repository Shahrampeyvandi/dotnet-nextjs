using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using testttt.Application.Interfaces;

namespace testttt.Application.Services;

/// <summary>
/// SMS service implementation using Kavehnegar provider
/// </summary>
public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _senderNumber;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        
        // Get Kavehnegar configuration from appsettings
        _apiKey = _configuration["Sms:Kavehnegar:ApiKey"] ?? throw new InvalidOperationException("Sms:Kavehnegar:ApiKey is not configured");
        _senderNumber = _configuration["Sms:Kavehnegar:SenderNumber"] ?? throw new InvalidOperationException("Sms:Kavehnegar:SenderNumber is not configured");
        
        // Set base address for Kavehnegar API
        _httpClient.BaseAddress = new Uri("https://api.kavenegar.com");
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            // Normalize phone number (remove spaces, ensure it starts with country code)
            var normalizedPhone = NormalizePhoneNumber(phoneNumber);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("receptor", normalizedPhone),
                new KeyValuePair<string, string>("sender", _senderNumber),
                new KeyValuePair<string, string>("message", message)
            });

            var response = await _httpClient.PostAsync($"/v1/{_apiKey}/sms/send.json", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("SMS sent successfully to {PhoneNumber}. Response: {Response}", normalizedPhone, responseContent);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send SMS to {PhoneNumber}. Status: {Status}, Response: {Response}", 
                    normalizedPhone, response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string otpCode)
    {
        // Customize OTP message template
        var message = $"کد تایید شما: {otpCode}\nاین کد تا 5 دقیقه معتبر است.";
        return await SendSmsAsync(phoneNumber, message);
    }

    private string NormalizePhoneNumber(string phoneNumber)
    {
        // Remove all non-digit characters
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // If it doesn't start with country code, add Iran's country code (98)
        if (!digits.StartsWith("98"))
        {
            // If it starts with 0, replace with 98
            if (digits.StartsWith("0"))
            {
                digits = "98" + digits.Substring(1);
            }
            else
            {
                digits = "98" + digits;
            }
        }
        
        return digits;
    }
}


namespace testttt.Application.DTOs;

public class RequestOtpDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Purpose { get; set; } = "Registration"; // Default to Registration
}

public class VerifyOtpDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Purpose { get; set; } = "Registration";
}

public class RegisterWithOtpDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class RequestOtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 5;
}

public class VerifyOtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}


namespace testttt.Domain.Entities;

/// <summary>
/// Entity برای ذخیره لاگ‌های Serilog در database
/// </summary>
public class Log
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? MessageTemplate { get; set; }
    public string Level { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; } // JSON format
    public string? LogEvent { get; set; } // JSON format
}


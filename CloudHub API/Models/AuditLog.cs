namespace CloudHub_API.Models;

public class AuditLog
{
    public string? EventType { get; set; }
    public string? Timestamp { get; set; }
    public string? Details { get; set; }
    public string? Level { get; set; }
}
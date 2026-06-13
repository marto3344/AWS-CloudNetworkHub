using CloudHub.Shared.Models;

namespace CloudHub_API.Services;

public interface IAuditLogService
{
    Task LogAsync(string eventType, string details, string level = "Info");
    Task<IEnumerable<AuditLog>> GetLogsAsync(string? level = null, int limit = 50);
}
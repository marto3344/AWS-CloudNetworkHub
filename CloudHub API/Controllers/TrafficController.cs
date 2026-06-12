using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;
[ApiController]
[Route("api/traffic")]
public class TrafficController (
    ITrafficService trafficService,
    IAuditLogService auditLogService,
    ILogger<TrafficService> logger) : ControllerBase
{
    [HttpGet("metrics")]
    public async Task<IActionResult> GetNetworkMetrics([FromQuery] int hours = 24)
    {
        try
        {
            var metrics = await trafficService.GetNetworkMetricsAsync(hours);
            return Ok(metrics);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetNetworkMetrics endpoint");
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs([FromQuery] string? level = null, [FromQuery] int limit = 50)
    {
        try
        {
            var logs = await auditLogService.GetLogsAsync(level, limit);
            return Ok(logs);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetAuditLogs endpoint");
            return StatusCode(500, e.Message);
        }
    }
}
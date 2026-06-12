using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;
[ApiController]
[Route("api/traffic")]
public class TrafficController(ITrafficService trafficService, ILogger<TrafficService> logger) : ControllerBase
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
}
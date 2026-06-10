using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/system")]
public class SystemStatusController (ISystemStatusService statusService, ILogger<SystemStatusController> logger) : ControllerBase
{
    [HttpGet("aws-status")]
    public async Task<IActionResult> GetAwsStatus()
    {
        try
        {
            var status = await statusService.GetAwsStatusAsync();
            return Ok(status);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetAwsStatus endpoint");
            return StatusCode(500, e.Message);
        }
    }
}
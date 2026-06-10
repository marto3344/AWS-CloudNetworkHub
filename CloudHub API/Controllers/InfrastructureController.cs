using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfrastructureController (IEc2MonitorService ec2MonitorService, ILogger<InfrastructureController> logger) : ControllerBase
{
    
    [HttpGet("instances")]
    public async Task<IActionResult> GetInstances()
    {
        try
        {
            var instances = await ec2MonitorService.GetInstancesAsync();
            return Ok(instances);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetInstances endpoint");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPost("instances/{id}/stop")]
    public async Task<IActionResult> StopInstance(string id)
    {
        try
        {
            var result = await ec2MonitorService.StopInstanceAsync(id);
            if (result)
                return Ok(new { message = $"Instance {id} is stopping." });
                
            return BadRequest(new { error = $"Could not stop instance {id}." });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
    
    [HttpPost("instances/{id}/reboot")]
    public async Task<IActionResult> RebootInstance(string id)
    {
        try
        {
            var result = await ec2MonitorService.RebootInstanceAsync(id);
            if (result)
                return Ok(new { message = $"Reboot command sent to instance {id}." });
                
            return BadRequest(new { error = $"Could not reboot instance {id}." });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
    
    [HttpPost("instances/{id}/start")]
    public async Task<IActionResult> StartInstance(string id)
    {
        try
        {
            await ec2MonitorService.StartInstanceAsync(id);
            return Ok(new { message = $"Instance {id} is starting." });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in StartInstance endpoint");
            return StatusCode(500, e.Message);
        }
    }
    
}
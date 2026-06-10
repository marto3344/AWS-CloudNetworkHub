using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfrastructureController (IEc2MonitorService ec2MonitorService) : ControllerBase
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
            return StatusCode(500, e.Message);
        }
    }
    
    
}
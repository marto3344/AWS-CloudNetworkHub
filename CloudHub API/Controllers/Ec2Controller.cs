using Amazon.EC2;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Ec2Controller (IAmazonEC2 ec2Client) : ControllerBase
{
    
    [HttpGet("instances")]
    public async Task<IActionResult> GetInstances()
    {
        try
        {
            var req = new DescribeInstancesRequest();
            var res = await ec2Client.DescribeInstancesAsync(req);
            return Ok(res);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"There was error in communication with AWS: {e.Message}");
        }
    }
    
    
}
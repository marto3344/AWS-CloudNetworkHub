using Amazon.EC2;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VpcController (IAmazonEC2 ec2Client) : ControllerBase
{
    [HttpGet("vpcs")]
    public async Task<IActionResult> GetVpcs()
    {
        try
        {
            // Заявка за извличане на VPC-тата
            var request = new DescribeVpcsRequest();
            var response = await ec2Client.DescribeVpcsAsync(request);

            var vpcs = response.Vpcs.Select(v => new
            {
                VpcId = v.VpcId,
                CidrBlock = v.CidrBlock,
                State = v.State.Value,
                IsDefault = v.IsDefault,
                Name = v.Tags?.FirstOrDefault(t => t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value ?? "No Name"
            }).ToList();

            return Ok(vpcs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Грешка при извличане на VPC-та: {ex.Message}");
        }
    }
}
using CloudHub_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHub_API.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityAuditController (ISecurityAuditService securityAuditService, ILogger<SecurityAuditController> logger) : ControllerBase
{
    [HttpGet("vpcs")]
    public async Task<IActionResult> GetVpcs()
    {
        try
        {
            var vpcs = await securityAuditService.GetVpcsAsync();
            return Ok(vpcs);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetVpcs endpoint");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("audit")]
    public async Task<IActionResult> GetSecurityAudit()
    {
        try
        {
            var audit = await securityAuditService.GetSecurityAuditAsync();
            return Ok(audit);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetSecurityAudit endpoint");
            return StatusCode(500, e.Message);
        }
    }
}
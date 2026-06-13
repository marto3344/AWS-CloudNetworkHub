using CloudHub.Shared.Models;

namespace CloudHub_API.Services;

public interface ISecurityAuditService
{
    Task<IEnumerable<VpcInfo>> GetVpcsAsync();
    Task<IEnumerable<SecurityGroupAudit>> GetSecurityAuditAsync();
}
namespace CloudHub_API.Models;

public class SecurityGroupAudit
{
    public string GroupId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string VpcId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public bool IsDangerous { get; set; }
    public List<string> Warnings { get; set; } = new();
}
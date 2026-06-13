namespace CloudHub.Shared.Models;

public class SubnetDto
{
    public string Name {get; set;} = string.Empty;
    public string SubnetId {get; set;} = string.Empty;
    public string CidrBlock {get; set;} = string.Empty;
    public string AvailabilityZone {get; set;} = string.Empty;
    public bool? IsPublic {get; set;}
    public int? AvailableIpAddressCount  {get; set;} 
}
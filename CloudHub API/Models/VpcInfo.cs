namespace CloudHub_API.Models;

public class VpcInfo
{
    public string VpcId { get; set; } = string.Empty;
    public string Name { get; set; } = "Unnamed VPC";
    public string CidrBlock { get; set; } = string.Empty; 
    public bool? IsDefault { get; set; }
    public string State { get; set; } = string.Empty; 
    public List<SubnetDto> Subnets { get; set; } = new();
}
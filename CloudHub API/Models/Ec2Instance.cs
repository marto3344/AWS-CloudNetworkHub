namespace CloudHub_API.Models;

public class Ec2Instance
{
    public string InstanceId { get; set; } = string.Empty;
    public string Name { get; set; } = "Unnamed Instance";
    public string Status { get; set; } = string.Empty; 
    public string InstanceType { get; set; } = string.Empty; 
    public string? PublicIpAddress { get; set; }
    public string? PrivateIpAddress { get; set; }
}
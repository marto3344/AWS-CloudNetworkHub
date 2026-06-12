namespace CloudHub_API.Models;

public class DangerousRule
{
    public string Protocol { get; set; } = string.Empty;
    public int FromPort { get; set; }
    public int ToPort { get; set; }
    public string CidrRange { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
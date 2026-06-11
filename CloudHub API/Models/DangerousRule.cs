namespace CloudHub_API.Models;

public class DangerousRule
{
    public string Protocol { get; set; }
    public int FromPort { get; set; }
    public int ToPort { get; set; }
    public string CidrRange { get; set; }
    public string Reason { get; set; }
}
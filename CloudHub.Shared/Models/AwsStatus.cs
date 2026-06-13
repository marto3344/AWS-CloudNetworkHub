namespace CloudHub.Shared.Models;

public class AwsStatus
{
    public bool IsConnected { get; set; }
    public string? AccountId { get; set; } 
    public string? UserId { get; set; } 
    public string? Message { get; set; }
}
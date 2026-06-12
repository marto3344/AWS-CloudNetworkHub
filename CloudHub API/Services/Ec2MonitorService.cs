using Amazon.EC2;
using Amazon.EC2.Model;
using CloudHub_API.Models;

namespace CloudHub_API.Services;

public class Ec2MonitorService (
    IAmazonEC2 ec2Client,
    IAuditLogService auditLogService,
    ILogger<Ec2MonitorService> logger) : IEc2MonitorService
{
    public async Task<IEnumerable<Ec2Instance>> GetInstancesAsync()
    {
        DescribeInstancesResponse res; 
        try
        {
            var req = new DescribeInstancesRequest();
            res = await ec2Client.DescribeInstancesAsync(req);
        }
        catch (AmazonEC2Exception awsEx)
        {
            logger.LogError(awsEx, $"AWS EC2 Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to fetch data from AWS due to a provider error.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An unexpected error occurred while fetching EC2 instances. {ex.Message}");
            throw new Exception("An internal error occurred. Please try again later.");
        }
        
        var instances = new List<Ec2Instance>();
        if (res?.Reservations == null)
            return instances;
        
        foreach (var reservation in res.Reservations)
        {
            foreach (var instance in reservation.Instances)
            {
                var nameTag = instance.Tags?
                    .FirstOrDefault(t => t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value;
                instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    Name = nameTag ?? "Unnamed Instance",
                    Status = instance.State?.Name ?? "Unknown",
                    InstanceType = instance.InstanceType ?? "Unknown",
                    PublicIpAddress = instance.PublicIpAddress,
                    PrivateIpAddress = instance.PrivateIpAddress
                });
            }
        }
        return instances;
    }

    public async Task<bool> StopInstanceAsync(string instanceId)
    {
        try
        {
            var req = new StopInstancesRequest { InstanceIds = [instanceId] };
            var res = await ec2Client.StopInstancesAsync(req);
            bool isStopping = res.StoppingInstances.Any();
            if (isStopping)
            {
                await auditLogService.LogAsync("StopInstance", $"Instance {instanceId} stopped.", "Warning");
            }
            return isStopping;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to stop EC2 instance {instanceId}. {ex.Message}");
            throw new Exception("Could not execute stop command.");
        }
    }

    public async Task<bool> RebootInstanceAsync(string instanceId)
    {
        try
        {
            var req = new RebootInstancesRequest { InstanceIds = [instanceId] };
            var res = await ec2Client.RebootInstancesAsync(req);
            bool isRebooting = res.HttpStatusCode == System.Net.HttpStatusCode.OK;
            if(isRebooting)
                await auditLogService.LogAsync("RebootInstance", $"Instance {instanceId} rebooted.", "Warning");
            return isRebooting;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to reboot EC2 instance {instanceId}. {ex.Message}");
            throw new Exception("Could not execute reboot command.");
        }
    }
    
    public async Task StartInstanceAsync(string instanceId)
    {
        try
        {
            var req = new StartInstancesRequest(){ InstanceIds = [instanceId] };
            var res = await ec2Client.StartInstancesAsync(req);
            await auditLogService.LogAsync("StartInstance", $"Instance {instanceId} started.", "Info");
        }
        catch (AmazonEC2Exception awsEx)
        {
            logger.LogError(awsEx, $"AWS EC2 Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to start instance due to a provider error.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to start EC2 instance {instanceId}. {ex.Message}");
            throw new Exception("Could not execute start command.");
        }
    }
}
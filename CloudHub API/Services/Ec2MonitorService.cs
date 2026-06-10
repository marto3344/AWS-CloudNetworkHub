using Amazon.EC2;
using Amazon.EC2.Model;
using CloudHub_API.Models;

namespace CloudHub_API.Services;

public class Ec2MonitorService (IAmazonEC2 ec2Client, ILogger<Ec2MonitorService> logger) : IEc2MonitorService
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
        foreach (var reservation in res.Reservations)
        {
            foreach (var instance in reservation.Instances)
            {
                var nameTag = instance.Tags
                    .FirstOrDefault(t => t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value;
                instances.Add(new Ec2Instance
                {
                    InstanceId = instance.InstanceId,
                    Name = nameTag ?? "Unnamed Instance",
                    Status = instance.State.Name,
                    InstanceType = instance.InstanceType,
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
            return res.StoppingInstances.Any();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to stop EC2 instance {InstanceId}", instanceId);
            throw new Exception("Could not execute stop command.");
        }
    }

    public async Task<bool> RebootInstanceAsync(string instanceId)
    {
        try
        {
            var req = new RebootInstancesRequest { InstanceIds = [instanceId] };
            var res = await ec2Client.RebootInstancesAsync(req);
            return res.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reboot EC2 instance {InstanceId}", instanceId);
            throw new Exception("Could not execute reboot command.");
        }
    }
}
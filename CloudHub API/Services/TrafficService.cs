using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using CloudHub.Shared.Models;

namespace CloudHub_API.Services;

public class TrafficService(
    IAmazonCloudWatch cloudWatchClient,
    IEc2MonitorService ec2MonitorService,
    ILogger<TrafficService> logger) :ITrafficService
{
    public async Task<IEnumerable<NetworkMetrics>> GetNetworkMetricsAsync(int hours)
    {
        try
        {
            var instances = await ec2MonitorService.GetInstancesAsync();

            var to = DateTime.UtcNow;
            var from = to.AddHours(-hours);

            var tasks = instances.Select(i => GetInstanceMetricsAsync(i, from, to));
            return await Task.WhenAll(tasks);
        }
        catch (AmazonCloudWatchException awsEx)
        {
            logger.LogError(awsEx, $"AWS CloudWatch Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to fetch metrics from AWS.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching network metrics.");
            throw new Exception("An internal error occurred. Please try again later.");
        }
    }

    public async Task<NetworkMetrics> GetInstanceMetricsAsync(Ec2Instance instance, DateTime from, DateTime to)
    {
       
        var queries = GetMetricQueries(instance.InstanceId, from, to);
        var req = new GetMetricDataRequest
        {
            MetricDataQueries = queries,
            StartTime = from,
            EndTime = to
        };
        
        var res = await cloudWatchClient.GetMetricDataAsync(req);
        
        double GetValue(string id) =>
            res.MetricDataResults.FirstOrDefault(r => r.Id == id)?.Values?.FirstOrDefault() ?? 0;

        return new NetworkMetrics
        {
            InstanceId = instance.InstanceId,
            InstanceName = instance.Name,
            CpuUtilization = GetValue("cpuutilization"),
            NetworkIn = GetValue("networkin"),
            NetworkOut = GetValue("networkout"),
            NetworkPacketsIn = GetValue("networkpacketsin"),
            NetworkPacketsOut = GetValue("networkpacketsout"),
            DiskReadBytes = GetValue("diskreadbytes"),
            DiskWriteBytes = GetValue("diskwritebytes"),
            DiskReadOps = GetValue("diskreadops"),
            DiskWriteOps = GetValue("diskwriteops"),
            From = from,
            To = to
        };
    }

    private List<MetricDataQuery> GetMetricQueries(string instanceId, DateTime from, DateTime to)
    {
        var metricNames = new[]
        {
            "cpuutilization",
            "networkin", "networkout",
            "networkpacketsin", "networkpacketsout",
            "diskreadbytes", "diskwritebytes",
            "diskreadops", "diskwriteops"
        };

        return metricNames.Select(metricName => new MetricDataQuery
        {
            Id = metricName, 
            MetricStat = new MetricStat
            {
                Metric = new Metric
                {
                    Namespace = "AWS/EC2",
                    MetricName = metricName,
                    Dimensions = [new Dimension { Name = "InstanceId", Value = instanceId }]
                },
                Period = (int)(to - from).TotalSeconds,
                Stat = "Sum"
            }
        }).ToList();
    }
}
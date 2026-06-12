using CloudHub_API.Models;

namespace CloudHub_API.Services;

public interface ITrafficService
{
    Task<IEnumerable<NetworkMetrics>> GetNetworkMetricsAsync(int hours);
    Task<NetworkMetrics> GetInstanceMetricsAsync(Ec2Instance instance, DateTime from, DateTime to);
}
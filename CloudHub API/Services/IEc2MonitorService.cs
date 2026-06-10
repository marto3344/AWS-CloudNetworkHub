using CloudHub_API.Models;

namespace CloudHub_API.Services;

public interface IEc2MonitorService
{
    Task<IEnumerable<Ec2Instance>> GetInstancesAsync();
}
using CloudHub.Shared.Models;

namespace CloudHub_API.Services;

public interface ISystemStatusService
{
    Task<AwsStatus> GetAwsStatusAsync();
}
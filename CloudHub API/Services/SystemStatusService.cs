using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using CloudHub_API.Models;

namespace CloudHub_API.Services;

public class SystemStatusService (IAmazonSecurityTokenService stsClient, ILogger<SystemStatusService> logger) :ISystemStatusService
{
    public async Task<AwsStatus> GetAwsStatusAsync()
    {
        try
        {
            var res = await stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());

            return new AwsStatus
            {
                IsConnected = true,
                AccountId = res.Account,
                UserId = res.UserId, 
                Message = "Successfully connected to AWS."
            };
        }
        catch (AmazonSecurityTokenServiceException awsEx)
        {
            logger.LogError(awsEx, $"AWS STS Error: {awsEx.Message}");
            return new AwsStatus
            {
                IsConnected = false,
                Message = $"AWS authentication failed: {awsEx.Message}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during AWS status check.");
            return new AwsStatus
            {
                IsConnected = false,
                Message = "An internal error occurred while checking AWS status."
            };
        }
    }
}
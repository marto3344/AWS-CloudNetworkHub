using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CloudHub_API.Models;

namespace CloudHub_API.Services;

public class AuditLogService (IAmazonDynamoDB dynamoDbClient, ILogger<AuditLogService> logger) : IAuditLogService
{
    private const string TableName = "CloudHubAuditLogs";
    public async Task LogAsync(string eventType, string details, string level = "Info")
    {
        try
        {
            var req = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "EventType", new AttributeValue { S = eventType } },
                    { "Timestamp", new AttributeValue { S = DateTime.UtcNow.ToString("o") } },
                    { "Details", new AttributeValue { S = details } },
                    { "Level", new AttributeValue { S = level } }
                }
            };

            await dynamoDbClient.PutItemAsync(req);
        }
        catch (AmazonDynamoDBException awsEx)
        {
            logger.LogError(awsEx, $"DynamoDB Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to write audit log to DynamoDB.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while writing audit log.");
            throw new Exception("An internal error occurred. Please try again later.");
        }
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync(string? level = null, int limit = 50)
    {
        try
        {
            var req = new ScanRequest
            {
                TableName = TableName,
                Limit = limit
            };
            
            if (!string.IsNullOrEmpty(level))
            {
                req.FilterExpression = "#lvl = :level";
                req.ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#lvl", "Level" } 
                };
                req.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":level", new AttributeValue { S = level } }
                };
            }

            var res = await dynamoDbClient.ScanAsync(req);

            return res.Items.Select(item => new AuditLog
            {
                EventType = item["EventType"].S,
                Timestamp = item["Timestamp"].S,
                Details = item["Details"].S,
                Level = item["Level"].S
            });
        }
        catch (AmazonDynamoDBException awsEx)
        {
            logger.LogError(awsEx, $"DynamoDB Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to fetch audit logs from DynamoDB.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching audit logs.");
            throw new Exception("An internal error occurred. Please try again later.");
        }
    }
}
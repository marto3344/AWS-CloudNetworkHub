using Amazon.EC2;
using Amazon.EC2.Model;
using CloudHub_API.Models;

namespace CloudHub_API.Services;

public class SecurityAuditService (IAmazonEC2 ec2Client, ILogger<SecurityAuditService> logger) : ISecurityAuditService
{
    private static readonly int[] DangerousPorts = [22, 3389, 1433, 3306, 5432, 27017];
    
    public async Task<IEnumerable<VpcInfo>> GetVpcsAsync()
    {
        try
        {
            var vpcsRes = await ec2Client.DescribeVpcsAsync(new DescribeVpcsRequest());
            var subnetsRes = await ec2Client.DescribeSubnetsAsync(new DescribeSubnetsRequest());
            
            return vpcsRes.Vpcs.Select(vpc => new VpcInfo
            {
                VpcId = vpc.VpcId,
                Name = GetVpcName(vpc),
                CidrBlock = vpc.CidrBlock,
                IsDefault = vpc.IsDefault,
                State = vpc.State.Value,
                Subnets = GetVpcSubnets(subnetsRes, vpc)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching VPCs.");
            throw new Exception("An internal error occurred. Please try again later.");
        }
    }
    
    public async Task<IEnumerable<SecurityGroupAudit>> GetSecurityAuditAsync()
    {
        try
        {
            var req = new DescribeSecurityGroupsRequest();
            var res = await ec2Client.DescribeSecurityGroupsAsync(req);
            var audits = new List<SecurityGroupAudit>();

            foreach (var sg in res.SecurityGroups)
            {
                audits.Add(AuditSecurityGroup(sg));
            }
            return audits;
        }
        catch (AmazonEC2Exception awsEx)
        {
            logger.LogError(awsEx, $"AWS EC2 Error: {awsEx.Message}. Code: {awsEx.ErrorCode}");
            throw new Exception("Unable to fetch Security Groups from AWS.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching Security Groups.");
            throw new Exception("An internal error occurred. Please try again later.");
        }
    }

    private SecurityGroupAudit AuditSecurityGroup(SecurityGroup sg)
    {
        var dangerousRules = new List<DangerousRule>();

        foreach (var rule in sg.IpPermissions ?? [])
        {
            ScanRule(rule, dangerousRules);
        }
        return new SecurityGroupAudit
        {
            GroupId = sg.GroupId,
            GroupName = sg.GroupName,
            VpcId = sg.VpcId,
            Description = sg.Description,
            IsDangerous = dangerousRules.Any(),
            DangerousRules = dangerousRules
        };
    }

    private void ScanRule(IpPermission rule, List<DangerousRule> dangerousRules)
    {
        foreach (var range in (rule.Ipv4Ranges ?? []).Where(r => r.CidrIp == "0.0.0.0/0"))
        {
            CheckRange(rule,range.CidrIp, dangerousRules);
        }
        foreach (var range in (rule.Ipv6Ranges ?? []).Where( r => r.CidrIpv6 == "::/0"))
        {
            CheckRange(rule,range.CidrIpv6, dangerousRules);
        }
    }

    private void CheckRange(IpPermission rule, string cidr, List<DangerousRule> dangerousRules)
    {
        if (rule.IpProtocol == "-1")
        {
            dangerousRules.Add(new DangerousRule
            {
                Protocol = "All Traffic",
                FromPort = 0,
                ToPort = 65535,
                CidrRange = cidr,
                Reason = "All ports and protocols are open to the world"
            });
            return;
        }
        ScanPorts(rule, cidr, dangerousRules);
    }
    private void ScanPorts(
        IpPermission rule,
        string cidr,
        List<DangerousRule> 
            dangerousRules)
    {
        foreach (var port in DangerousPorts)
        {
            if (port >= rule.FromPort && port <= rule.ToPort)
            {
                dangerousRules.Add(new DangerousRule
                {
                    Protocol = rule.IpProtocol,
                    FromPort = port,
                    ToPort = port,
                    CidrRange = cidr,
                    Reason = GetDangerReason(port)
                });
            }
        }
    }
    
    private string GetDangerReason(int port) => port switch
    {
        22 => "SSH port open to the world",
        3389 => "RDP port open to the world",
        1433 => "MSSQL port open to the world",
        3306 => "MySQL port open to the world",
        5432 => "PostgreSQL port open to the world",
        27017 => "MongoDB port open to the world",
        _ => "Sensitive port open to the world"
    };

    private string GetVpcName(Vpc vpc)
    {
         return vpc.
             Tags?.
             FirstOrDefault(t => 
                 t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value ?? "Unnamed VPC";
    }
    
    private string GetSubnetName(Subnet s)
    {
        return s.
            Tags?.
            FirstOrDefault(t =>
            t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value ?? "Unnamed Subnet";
    }
    
    private List<SubnetDto> GetVpcSubnets(DescribeSubnetsResponse subnetsRes, Vpc vpc)
    {
        return subnetsRes.Subnets
            .Where(s => s.VpcId == vpc.VpcId)
            .Select(s => new SubnetDto
            {
                SubnetId = s.SubnetId,
                Name = GetSubnetName(s),
                CidrBlock = s.CidrBlock,
                AvailabilityZone = s.AvailabilityZone,
                IsPublic = s.MapPublicIpOnLaunch,
                AvailableIpAddressCount = s.AvailableIpAddressCount
            }).ToList();
    }
}
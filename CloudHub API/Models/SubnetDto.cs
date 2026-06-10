namespace CloudHub_API.Models;

public record SubnetDto(
    string  Name, 
    string  SubnetId, 
    string  CidrBlock, 
    string  AvailabilityZone, 
    int AvailableIpAddressCount
);
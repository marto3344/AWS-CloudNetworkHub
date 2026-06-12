namespace CloudHub_API.Models;

public class NetworkMetrics
{
    public string InstanceId { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
    
    public double CpuUtilization { get; set; }      
   
    public double NetworkIn { get; set; }            
    public double NetworkOut { get; set; }          
    public double NetworkPacketsIn { get; set; }     
    public double NetworkPacketsOut { get; set; }    
    
    public double DiskReadBytes { get; set; }        
    public double DiskWriteBytes { get; set; }       
    public double DiskReadOps { get; set; }          
    public double DiskWriteOps { get; set; }         
    
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
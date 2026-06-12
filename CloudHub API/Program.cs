using Amazon.EC2;
using Amazon.SecurityToken;
using CloudHub_API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var awsOptions = builder.Configuration.GetAWSOptions();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonEC2>();
builder.Services.AddAWSService<IAmazonSecurityTokenService>();
builder.Services.AddScoped<IEc2MonitorService, Ec2MonitorService>();
builder.Services.AddScoped<ISystemStatusService, SystemStatusService>();
builder.Services.AddScoped<ISecurityAuditService, SecurityAuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
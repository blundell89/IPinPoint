using IPinPoint.Api.IpLocations;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddIpLocationsFeature()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSwagger();
app.UseSwaggerUI();

IpLocationsEndpoints.Map(app);

app.Run();

public partial class Program { }
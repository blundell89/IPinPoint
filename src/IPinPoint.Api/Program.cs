using IPinPoint.Api.IpLocations;
using IPinPoint.Api.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

MongoDbConfigurator.Configure();

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services
    .AddOptions<MongoDbOptions>()
    .BindConfiguration("MongoDb")
    .ValidateDataAnnotations()
    .ValidateOnStart();

services
    .AddSingleton<IMongoClient>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<MongoDbOptions>>();
        var mongoClientSettings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        return new MongoClient(mongoClientSettings);
    })
    .AddSingleton<IMongoDatabase>(serviceProvider =>
        serviceProvider.GetRequiredService<IMongoClient>().GetDatabase("ipinpoint"))
    // ideally we would create indexes in a migration and exercise least privilege so that the application user doesn't have elevated permissions
    .AddHostedService<MongoDbHostedService>();

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
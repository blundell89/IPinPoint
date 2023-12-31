using System.Net;
using System.Text.Json;
using IPinPoint.Api.Domain;
using MongoDB.Driver;

namespace IPinPoint.Api.IntegrationTests;

public sealed class MongoDbHarness
{
    public MongoDbHarness(IServiceProvider serviceProvider)
    {
        IpLocations = serviceProvider.GetRequiredService<IMongoDatabase>()
            .GetCollection<IpLocation>("ipLocations");
    }
    
    public IMongoCollection<IpLocation> IpLocations { get; }
}

public sealed class WebHarness : IAsyncDisposable
{
    public IPinPointWebApplicationFactory Factory { get; } = new();
    
    public MongoDbHarness MongoDb => new(Factory.Services);

    public async Task<(HttpResponseMessage Response, string? Content)> GetSwaggerUi()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        var content = await response.Content.ReadAsStringAsync();
        if (content.Length is 0)
            return (response, null);
        return (response, content);
    }
    
    public async Task<(HttpStatusCode StatusCode, JsonDocument? Body)> GetSwaggerJson()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var content = await response.Content.ReadAsStreamAsync();
        if (content.Length is 0)
            return (response.StatusCode, null);
        return (response.StatusCode, await JsonDocument.ParseAsync(content));
    }
    
    public async Task<(HttpStatusCode StatusCode, JsonDocument? Body)> GetIpLocation(string ip)
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"/ip-locations/{ip}");
        
        var content = await response.Content.ReadAsStreamAsync();
        if (content.Length is 0)
            return (response.StatusCode, null);
        return (response.StatusCode, await JsonDocument.ParseAsync(content));
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}
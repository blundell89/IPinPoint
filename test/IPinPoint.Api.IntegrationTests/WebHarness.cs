using System.Net;
using System.Text.Json;

namespace IPinPoint.Api.IntegrationTests;

public sealed class WebHarness : IAsyncDisposable
{
    public IPinPointWebApplicationFactory Factory { get; } = new();

    public async Task<(HttpResponseMessage Response, string? Content)> GetSwaggerUi()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        if (!response.IsSuccessStatusCode)
            return (response, null);
        
        var content = await response.Content.ReadAsStringAsync();
        return (response, content);
    }
    
    public async Task<(HttpStatusCode StatusCode, JsonDocument? Body)> GetSwaggerJson()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        if (!response.IsSuccessStatusCode)
            return (response.StatusCode, null);
        
        var content = await response.Content.ReadAsStreamAsync();
        return (response.StatusCode, await JsonDocument.ParseAsync(content));
    }
    
    public async Task<(HttpStatusCode StatusCode, JsonDocument? Body)> GetIpLocation(string ip)
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"/ip-locations/{ip}");
        if (!response.IsSuccessStatusCode)
            return (response.StatusCode, null);
        
        var content = await response.Content.ReadAsStreamAsync();
        return (response.StatusCode, await JsonDocument.ParseAsync(content));
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}
using System.Net;
using System.Text.Json;

namespace IPinPoint.Api.IntegrationTests;

public sealed class WebHarness : IAsyncDisposable
{
    public WebHarness()
    {
        Factory = new IPinPointWebApplicationFactory();
    }

    public IPinPointWebApplicationFactory Factory { get; }
    
    public async Task<(HttpResponseMessage Response, string? Content)> GetSwaggerUi()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return (response, content);
        }

        return (response, null);
    }
    
    public async Task<(HttpStatusCode StatusCode, JsonDocument? Body)> GetSwaggerJson()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStreamAsync();
            return (response.StatusCode, await JsonDocument.ParseAsync(content));
        }

        return (response.StatusCode, null);
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}
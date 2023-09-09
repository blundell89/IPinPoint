using System.Net;
using System.Text.Json;

namespace IPinPoint.Api.IpLocations.FreeIpApi;

public class FreeIpApiHttpClient
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    private readonly HttpClient _httpClient;
    private readonly ILogger<FreeIpApiHttpClient> _logger;

    public FreeIpApiHttpClient(HttpClient httpClient, ILogger<FreeIpApiHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetIpResponse?> GetIpLocation(IPAddress ipAddress, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/json/{ipAddress}", cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
            return null;
        
        if (response.StatusCode is not HttpStatusCode.OK)
        {
            _logger.LogInformation("Unable to get IP location from downstream API. Status code: {StatusCode}",
                response.StatusCode);
            throw new InvalidOperationException("Unable to get IP location from downstream API");
        }

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var data = await JsonSerializer.DeserializeAsync<GetIpResponse>(stream, _serializerOptions, cancellationToken: cancellationToken);
        if (data is null)
            throw new InvalidOperationException("Failed to deserialize response");
            
        return data;
    }
}
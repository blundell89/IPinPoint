using FluentAssertions;
using System.Net;
using IPinPoint.Api.IntegrationTests.CustomAssertions;

namespace IPinPoint.Api.IntegrationTests.IpLocations;

public class GetIpLocationTests : IAsyncLifetime
{
    private readonly WebHarness _harness = new();
    
    [Fact]
    public async Task ShouldReturnBadRequestForInvalidIp()
    {
        var (statusCode, body) = await _harness.GetIpLocation("invalid");
        statusCode.Should().Be(HttpStatusCode.BadRequest);
        body!.Should().NotBeNull();
        body!.Should().BeProblemDetails("Validation error", 400, "Invalid IP address format");
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownIp()
    {
        const string ip = "1.1.1.1";
        var mockIpApiHttpHandler = _harness.Factory.Services.GetRequiredService<MockFreeIpApiHttpMessageHandler>();
        mockIpApiHttpHandler.AddResponse(
            req => req.Method == HttpMethod.Get &&
                   req.RequestUri!.AbsolutePath == $"/api/json/{ip}",
            req => new HttpResponseMessage(HttpStatusCode.NotFound));

        var (statusCode, body) = await _harness.GetIpLocation(ip);
        
        statusCode.Should().Be(HttpStatusCode.NotFound);
        body!.Should().BeNull();
        mockIpApiHttpHandler.Invocations.Should().HaveCount(1);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
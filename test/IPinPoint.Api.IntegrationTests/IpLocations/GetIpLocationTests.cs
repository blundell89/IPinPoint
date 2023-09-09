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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
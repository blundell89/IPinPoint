using System.Net;
using FluentAssertions;

namespace IPinPoint.Api.IntegrationTests.IpLocations;

public class GetIpLocationTests : IAsyncLifetime
{
    private readonly WebHarness _harness = new();
    
    [Fact]
    public async Task ShouldReturnNotFoundForInvalidIp()
    {
        var (statusCode, body) = await _harness.GetIpLocation("invalid");
        statusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
using System.Net;
using FluentAssertions;
using IPinPoint.Api.IntegrationTests.CustomAssertions;

namespace IPinPoint.Api.IntegrationTests.Swagger;

public class SwaggerTests : IAsyncLifetime
{
    private readonly WebHarness _harness = new();
    
    [Fact]
    public async Task ShouldRenderSwaggerUi()
    {
        var (response, content) = await _harness.GetSwaggerUi();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
        response.Content.Headers.ContentType.CharSet.Should().Be("utf-8");
        content.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task ShouldRenderSwaggerJson()
    {
        var (statusCode, body) = await _harness.GetSwaggerJson();
        statusCode.Should().Be(HttpStatusCode.OK);
        body!.Should().NotBeNull();
        body!.RootElement.GetProperty("info").GetProperty("title").GetString().Should().StartWith("IPinPoint");
        body.RootElement.GetProperty("info").GetProperty("version").GetString().Should().Be("1.0");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
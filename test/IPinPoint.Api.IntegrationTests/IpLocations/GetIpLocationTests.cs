using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
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
    
    [Fact]
    public async Task ShouldReturnIpLocationDataWhenMatchedFromFreeIpApi()
    {
        const string ip = "1.1.1.1";
        var mockIpApiHttpHandler = _harness.Factory.Services.GetRequiredService<MockFreeIpApiHttpMessageHandler>();
        var freeIpApiResponse = FreeIpApiResponses.SuccessResponse(ip);
        var freeIpApiContent = freeIpApiResponse.RootElement;
        mockIpApiHttpHandler.AddResponse(
            req => req.Method == HttpMethod.Get &&
                   req.RequestUri!.AbsolutePath == $"/api/json/{ip}",
            req =>
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(freeIpApiContent.ToString()!);
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
                return responseMessage;
            });

        var (statusCode, body) = await _harness.GetIpLocation(ip);
        
        statusCode.Should().Be(HttpStatusCode.OK);
        body!.Should().NotBeNull();
        var content = body!.RootElement;
        
        content.GetProperty("ipAddress").GetString().Should().Be(ip);
        content.GetProperty("latitude").GetDouble().Should().Be(
            freeIpApiContent.GetProperty("latitude").GetDouble());
        content.GetProperty("longitude").GetDouble().Should().Be(
            freeIpApiContent.GetProperty("longitude").GetDouble());
        content.GetProperty("country").GetString().Should().Be(
            freeIpApiContent.GetProperty("countryName").GetString());
        content.GetProperty("countryCode").GetString().Should().Be(
            freeIpApiContent.GetProperty("countryCode").GetString());
        content.GetProperty("postalCode").GetString().Should().Be(
            freeIpApiContent.GetProperty("zipCode").GetString());
        content.GetProperty("city").GetString().Should().Be(
            freeIpApiContent.GetProperty("cityName").GetString());
        content.GetProperty("region").GetString().Should().Be(
            freeIpApiContent.GetProperty("regionName").GetString());
        
        mockIpApiHttpHandler.Invocations.Should().HaveCount(1);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
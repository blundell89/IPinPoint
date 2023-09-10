using FluentAssertions;
using System.Net;
using System.Net.Sockets;
using IPinPoint.Api.Domain;
using IPinPoint.Api.IntegrationTests.CustomAssertions;
using IPinPoint.Api.Tests.Shared;
using IPinPoint.Api.Tests.Shared.IpLocations;
using MongoDB.Driver;

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
        var ip = RandomIpGenerator.Generate().ToString();
        var mockIpApiHttpHandler = _harness.Factory.Services.GetRequiredService<MockFreeIpApiHttpMessageHandler>();
        var freeIpApiResponse = FreeIpApiResponses.SuccessResponse(ip);
        var freeIpApiContent = freeIpApiResponse.RootElement;
        mockIpApiHttpHandler.AddSuccessResponse(
            req => req.Method == HttpMethod.Get &&
                   req.RequestUri!.AbsolutePath == $"/api/json/{ip}",
            freeIpApiResponse);

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
    
    [Fact]
    public async Task ShouldStoreIpLocationDataWhenMatchedFromFreeIpApi()
    {
        var ip = RandomIpGenerator.Generate().ToString();
        var mockIpApiHttpHandler = _harness.Factory.Services.GetRequiredService<MockFreeIpApiHttpMessageHandler>();
        var freeIpApiResponse = FreeIpApiResponses.SuccessResponse(ip);
        var freeIpApiContent = freeIpApiResponse.RootElement;
        mockIpApiHttpHandler.AddSuccessResponse(
            req => req.Method == HttpMethod.Get &&
                   req.RequestUri!.AbsolutePath == $"/api/json/{ip}",
            freeIpApiResponse);

        var (statusCode, body) = await _harness.GetIpLocation(ip);

        statusCode.Should().Be(HttpStatusCode.OK);
        
        var ipAddressFilter = Builders<IpLocation>.Filter.Eq(nameof(IpLocation.IpAddress), ip);
        var ipLocationDocument = await _harness.MongoDb.IpLocations.Find(ipAddressFilter).FirstOrDefaultAsync();

        ipLocationDocument.Should().NotBeNull();
        ipLocationDocument!.IpAddress.ToString().Should().Be(ip);
        ipLocationDocument.Latitude.Should().Be(
            (float)freeIpApiContent.GetProperty("latitude").GetDouble());
        ipLocationDocument.Longitude.Should().Be(
            (float)freeIpApiContent.GetProperty("longitude").GetDouble());
        ipLocationDocument.Country.Should().Be(
            freeIpApiContent.GetProperty("countryName").GetString());
        ipLocationDocument.CountryCode.Should().Be(
            freeIpApiContent.GetProperty("countryCode").GetString());
        ipLocationDocument.PostalCode.Should().Be(
            freeIpApiContent.GetProperty("zipCode").GetString());
        ipLocationDocument.City.Should().Be(
            freeIpApiContent.GetProperty("cityName").GetString());
        ipLocationDocument.Region.Should().Be(
            freeIpApiContent.GetProperty("regionName").GetString());
    }
    
    [Fact]
    public async Task ShouldReturnOkWithAnIpV6()
    {
        var ip = RandomIpGenerator.Generate(AddressFamily.InterNetworkV6);
        var mockIpApiHttpHandler = _harness.Factory.Services.GetRequiredService<MockFreeIpApiHttpMessageHandler>();
        var freeIpApiResponse = FreeIpApiResponses.SuccessResponse(ip.ToString());
        mockIpApiHttpHandler.AddSuccessResponse(
            req => req.Method == HttpMethod.Get &&
                   req.RequestUri!.AbsolutePath == $"/api/json/{ip}",
            freeIpApiResponse);

        var (statusCode, body) = await _harness.GetIpLocation(ip.ToString());

        statusCode.Should().Be(HttpStatusCode.OK);
        
        var ipAddressFilter = Builders<IpLocation>.Filter.Eq(nameof(IpLocation.IpAddress), ip);
        var ipLocationDocument = await _harness.MongoDb.IpLocations.Find(ipAddressFilter).FirstOrDefaultAsync();

        ipLocationDocument.Should().NotBeNull();
        ipLocationDocument!.IpAddress.ToString().Should().Be(ip.ToString());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _harness.DisposeAsync();
    }
}
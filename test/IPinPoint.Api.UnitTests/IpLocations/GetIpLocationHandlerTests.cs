using System.Net;
using FluentAssertions;
using IPinPoint.Api.IpLocations;
using IPinPoint.Api.IpLocations.FreeIpApi;
using IPinPoint.Api.IpLocations.Persistence;
using IPinPoint.Api.Tests.Shared;
using IPinPoint.Api.Tests.Shared.IpLocations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace IPinPoint.Api.UnitTests.IpLocations;

public class GetIpLocationHandlerTests
{
    [Fact]
    public async Task ShouldUseCacheOnSubsequentRequestsAfterASuccessfulGet()
    {
        var ip = RandomIpGenerator.Generate();
        var mockFreeIpApiHttpMessageHandler = new MockFreeIpApiHttpMessageHandler();
        mockFreeIpApiHttpMessageHandler.AddSuccessResponse(_ => true, FreeIpApiResponses.SuccessResponse(ip.ToString()));
        var freeIpApiClient = new FreeIpApiHttpClient(new HttpClient(mockFreeIpApiHttpMessageHandler)
        {
            BaseAddress = new Uri("https://test.test", UriKind.Absolute)
        }, NullLogger<FreeIpApiHttpClient>.Instance);
        var handler = new GetIpLocationHandler(freeIpApiClient, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IpLocationRepository>());

        var request = new GetIpLocationHandler.GetIpLocation(ip);
        await handler.Handle(request, CancellationToken.None);
        await handler.Handle(request, CancellationToken.None);
        
        mockFreeIpApiHttpMessageHandler.Invocations.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ShouldBypassCacheWhenIpLocationDataNotFound()
    {
        var ip = RandomIpGenerator.Generate();
        var mockFreeIpApiHttpMessageHandler = new MockFreeIpApiHttpMessageHandler();
        mockFreeIpApiHttpMessageHandler.AddResponse(_ => true, _ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var freeIpApiClient = new FreeIpApiHttpClient(new HttpClient(mockFreeIpApiHttpMessageHandler)
        {
            BaseAddress = new Uri("https://test.test", UriKind.Absolute)
        }, NullLogger<FreeIpApiHttpClient>.Instance);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetIpLocationHandler(freeIpApiClient, memoryCache, Mock.Of<IpLocationRepository>());

        var request = new GetIpLocationHandler.GetIpLocation(ip);
        await handler.Handle(request, CancellationToken.None);
        await handler.Handle(request, CancellationToken.None);
        
        mockFreeIpApiHttpMessageHandler.Invocations.Should().HaveCount(2);
        memoryCache.TryGetValue(ip.ToString(), out _).Should().BeFalse();
    }
}
using System.Net;
using IPinPoint.Api.Domain;
using IPinPoint.Api.IpLocations.FreeIpApi;
using Microsoft.Extensions.Caching.Memory;

namespace IPinPoint.Api.IpLocations;

public class GetIpLocationHandler
{
    private readonly FreeIpApiHttpClient _freeIpApiHttpClient;
    private readonly IMemoryCache _cache;

    public GetIpLocationHandler(
        FreeIpApiHttpClient freeIpApiHttpClient,
        IMemoryCache cache)
    {
        _freeIpApiHttpClient = freeIpApiHttpClient;
        _cache = cache;
    }
    
    public async Task<Result> Handle(GetIpLocation request, CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress.ToString();

        if (_cache.TryGetValue(ipAddress, out IpLocation? cachedIpLocation))
            return new Match(cachedIpLocation!);
        
        var response = await _freeIpApiHttpClient.GetIpLocation(request.IpAddress, cancellationToken);
        
        if (response is null)
            return new NotFound();
        
        var ipLocation = new IpLocation(
            Guid.NewGuid(),
            request.IpAddress,
            response.Latitude,
            response.Longitude,
            response.CountryName,
            response.CountryCode,
            response.ZipCode,
            response.CityName,
            response.RegionName);
        
        _cache.Set(ipAddress, ipLocation, TimeSpan.FromHours(1));
        
        return new Match(ipLocation);
    }

    public record GetIpLocation(IPAddress IpAddress);

    public abstract record Result();
    
    public record Match(IpLocation IpLocation) : Result();
    
    public record NotFound() : Result();
}
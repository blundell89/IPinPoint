using System.Net;
using IPinPoint.Api.Domain;
using IPinPoint.Api.IpLocations.FreeIpApi;
using IPinPoint.Api.IpLocations.Persistence;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;

namespace IPinPoint.Api.IpLocations;

public class GetIpLocationHandler
{
    private static readonly TimeSpan OneHourTimeSpan = TimeSpan.FromHours(1);
    private const string CachePrefix = "ipLocation_";
    private readonly FreeIpApiHttpClient _freeIpApiHttpClient;
    private readonly IMemoryCache _cache;
    private readonly IpLocationRepository _repository;

    public GetIpLocationHandler(
        FreeIpApiHttpClient freeIpApiHttpClient,
        IMemoryCache cache,
        IpLocationRepository repository)
    {
        _freeIpApiHttpClient = freeIpApiHttpClient;
        _cache = cache;
        _repository = repository;
    }

    public async Task<Result> Handle(GetIpLocation request, CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress;

        if (_cache.TryGetValue(GenerateCacheKey(ipAddress), out IpLocation? cachedIpLocation))
            return new Match(cachedIpLocation!);

        var ipLocation = await _repository.Get(ipAddress, cancellationToken);

        if (ipLocation is not null)
        {
            SetCache(ipLocation);
            return new Match(ipLocation);
        }

        var response = await _freeIpApiHttpClient.GetIpLocation(request.IpAddress, cancellationToken);

        if (response is null)
            return new NotFound();

        ipLocation = new IpLocation(
            ObjectId.GenerateNewId(),
            request.IpAddress,
            response.Latitude,
            response.Longitude,
            response.CountryName,
            response.CountryCode,
            response.ZipCode,
            response.CityName,
            response.RegionName);

        SetCache(ipLocation);
        // we don't mind if the insert failed due to the IP already existing
        _ = await _repository.TryInsert(ipLocation, cancellationToken);

        return new Match(ipLocation);
    }

    private void SetCache(IpLocation ipLocation)
    {
        _cache.Set(GenerateCacheKey(ipLocation.IpAddress), ipLocation, OneHourTimeSpan);
    }

    private static string GenerateCacheKey(IPAddress ipAddress) => $"{CachePrefix}{ipAddress}";

    public record GetIpLocation(IPAddress IpAddress);

    public abstract record Result();

    public record Match(IpLocation IpLocation) : Result();

    public record NotFound() : Result();
}
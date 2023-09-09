using System.Net;
using IPinPoint.Api.Domain;
using IPinPoint.Api.IpLocations.FreeIpApi;

namespace IPinPoint.Api.IpLocations;

public class GetIpLocationHandler
{
    private readonly FreeIpApiHttpClient _freeIpApiHttpClient;

    public GetIpLocationHandler(FreeIpApiHttpClient freeIpApiHttpClient)
    {
        _freeIpApiHttpClient = freeIpApiHttpClient;
    }
    
    public async Task<Result> Handle(GetIpLocation request, CancellationToken cancellationToken)
    {
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
        
        return new Match(ipLocation);
    }

    public record GetIpLocation(IPAddress IpAddress);

    public abstract record Result();
    
    public record Match(IpLocation IpLocation) : Result();
    
    public record NotFound() : Result();
}
using System.Net;
using IPinPoint.Api.Domain;

namespace IPinPoint.Api.IpLocations;

public class GetIpLocationHandler
{
    public record GetIpLocation(IPAddress IpAddress);

    public abstract record Result();
    
    public record Match(IpLocation IpLocation) : Result();
    
    public record NotFound() : Result();
    
    public Task<Result> Handle(GetIpLocation request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result>(new NotFound());
    }
}
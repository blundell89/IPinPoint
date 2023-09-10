using System.Net;
using IPinPoint.Api.Domain;

namespace IPinPoint.Api.IpLocations.Persistence;

public interface IpLocationRepository
{
    Task<IpLocation?> Get(IPAddress ipAddress, CancellationToken cancellationToken);

    Task<InsertResult> TryInsert(IpLocation ipLocation, CancellationToken cancellationToken);
}
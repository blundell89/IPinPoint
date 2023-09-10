using System.Net;
using IPinPoint.Api.Domain;
using MongoDB.Driver;

namespace IPinPoint.Api.IpLocations.Persistence;

public class MongoIpLocationRepository : IpLocationRepository
{
    private readonly IMongoCollection<IpLocation> _ipLocationsCollection;

    public MongoIpLocationRepository(IMongoDatabase mongoDatabase)
    {
        _ipLocationsCollection = mongoDatabase.GetCollection<IpLocation>("ipLocations");
    }

    public async Task<IpLocation?> Get(IPAddress ipAddress, CancellationToken cancellationToken)
    {
        var ipAddressFilter = Builders<IpLocation>.Filter.Eq(nameof(IpLocation.IpAddress), ipAddress);
        return await _ipLocationsCollection
            .Find(ipAddressFilter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InsertResult> TryInsert(IpLocation ipLocation, CancellationToken cancellationToken)
    {
        try
        {
            await _ipLocationsCollection.InsertOneAsync(ipLocation, cancellationToken: cancellationToken);
            return new Inserted();
        }
        catch (MongoWriteException e) when (e.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return new AlreadyExists();
        }
    }
}
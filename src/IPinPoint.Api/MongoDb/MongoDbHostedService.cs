using IPinPoint.Api.Domain;
using MongoDB.Driver;

namespace IPinPoint.Api.MongoDb;

public class MongoDbHostedService : IHostedService
{
    private readonly IMongoCollection<IpLocation> _ipLocationCollection;

    public MongoDbHostedService(IMongoDatabase mongoDatabase)
    {
        _ipLocationCollection = mongoDatabase.GetCollection<IpLocation>("ipLocations");
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var key = Builders<IpLocation>.IndexKeys.Ascending(x => x.IpAddress);
        var options = new CreateIndexOptions
        {
            Unique = true
        };
        var model = new CreateIndexModel<IpLocation>(key, options);
        await _ipLocationCollection.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
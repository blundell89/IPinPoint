using IPinPoint.Api.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace IPinPoint.Api.MongoDb;

public static class MongoDbConfigurator
{
    private static readonly object _lock = new();
    private static bool _runOnce;
    
    public static void Configure()
    {
        if (_runOnce)
            return;

        lock (_lock)
        {
            if (_runOnce)
                return;
            
            BsonClassMap.RegisterClassMap<IpLocation>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.IpAddress)
                    .SetSerializer(new IPAddressSerializer());
            });
            
            _runOnce = true;
        }
    }
}
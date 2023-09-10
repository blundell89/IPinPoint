using IPinPoint.Api.IpLocations.FreeIpApi;
using IPinPoint.Api.IpLocations.Persistence;

namespace IPinPoint.Api.IpLocations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIpLocationsFeature(this IServiceCollection services)
    {
        services
            .AddScoped<GetIpLocationHandler>()
            .AddScoped<IpLocationRepository, MongoIpLocationRepository>()
            .AddMemoryCache()
            .AddFreeIpApi();

        return services;
    }
}
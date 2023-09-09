namespace IPinPoint.Api.IpLocations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIpLocationsFeature(this IServiceCollection services)
    {
        services.AddScoped<GetIpLocationHandler>();

        return services;
    }
}
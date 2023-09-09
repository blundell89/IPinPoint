using Microsoft.Extensions.Options;

namespace IPinPoint.Api.IpLocations.FreeIpApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFreeIpApi(this IServiceCollection services)
    {
        services.AddOptions<FreeIpApiOptions>()
            .BindConfiguration("FreeIpApi")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddHttpClient<FreeIpApiHttpClient>("FreeIpApi")
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<FreeIpApiOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseAddress, UriKind.Absolute);
            });

        return services;
    }
}
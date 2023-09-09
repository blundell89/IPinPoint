using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Http;

namespace IPinPoint.Api.IntegrationTests;

public class IPinPointWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<MockFreeIpApiHttpMessageHandlerHandler>();
            services.Configure<HttpClientFactoryOptions>("FreeIpApi", options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(
                    messageHandlerBuilder =>
                        messageHandlerBuilder.PrimaryHandler = 
                            messageHandlerBuilder.Services.GetRequiredService<MockFreeIpApiHttpMessageHandlerHandler>());
            });
        });
    }
}
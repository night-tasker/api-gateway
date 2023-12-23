using Ocelot.DependencyInjection;

namespace NightTasker.ApiGateway.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOcelot(this IServiceCollection services)
    {
        services.AddOcelot();
        return services;
    }
}
using NightTasker.ApiGateway.Constants;
using NightTasker.ApiGateway.Settings;
using Ocelot.DependencyInjection;

namespace NightTasker.ApiGateway.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RouteConfigurationSettings>(
            configuration.GetSection(nameof(RouteConfigurationSettings)));
        
        return services;
    }
    
    public static IServiceCollection ConfigureOcelot(this IServiceCollection services)
    {
        services.AddOcelot();
        return services;
    }

    public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsConstants.DefaultCorsPolicyName, defaultCorsOptions =>
            {
                defaultCorsOptions.AllowAnyOrigin();
                defaultCorsOptions.AllowAnyHeader();
                defaultCorsOptions.AllowAnyMethod();
            });
        });

        return services;
    }
}
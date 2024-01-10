using MMLib.SwaggerForOcelot.DependencyInjection;
using NightTasker.ApiGateway.Services;
using NightTasker.ApiGateway.Settings;

namespace NightTasker.ApiGateway.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static async Task<WebApplicationBuilder> AddConfigurationFiles(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
        });

        var routeConfigurationSettings = builder.Configuration
            .GetSection(nameof(RouteConfigurationSettings))
            .Get<RouteConfigurationSettings>()!;
        
        var ocelotRoutesService = new OcelotRoutesService(builder.Configuration, routeConfigurationSettings);
        var generatedFilesPaths = await ocelotRoutesService.GenerateOcelotFiles();

        builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
        
            foreach (var generatedFilePath in generatedFilesPaths)
            {
                config.AddJsonFile(generatedFilePath, true, true);
            }
                
            config.AddEnvironmentVariables();
        });

        builder.Configuration.AddOcelotWithSwaggerSupport(options =>
        {
            // options.FileOfSwaggerEndPoints = routeConfigurationSettings.SwaggerEndPoints.GeneratedFilePath;
            options.Folder = "GeneratedRoutes";
        });
        
        return builder;
    }
}
using NightTasker.ApiGateway.Services;

namespace NightTasker.ApiGateway.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddConfigurationFiles(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
        });
        
        var ocelotRoutesService = new OcelotRoutesService(builder.Configuration);
        ocelotRoutesService.ReplaceEnvironmentVariables().Wait();
        var generatedFilePath = builder.Configuration["RoutesConfiguration:FilePath:Generated"]!;

        builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile(generatedFilePath, true, true)
                .AddEnvironmentVariables();
        });
        
        return builder;
    }
}
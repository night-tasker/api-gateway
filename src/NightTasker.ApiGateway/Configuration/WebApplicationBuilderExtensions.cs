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
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables();
        });
        
        return builder;
    }
}
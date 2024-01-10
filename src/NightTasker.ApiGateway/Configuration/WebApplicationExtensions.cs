using Ocelot.Middleware;

namespace NightTasker.ApiGateway.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureOcelot(this WebApplication app)
    {
        app.UseSwaggerForOcelotUI(options =>
        {
            options.PathToSwaggerGenerator = "/swagger/docs";
            options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
        }).UseOcelot().Wait();
        return app;
    }
}
using Ocelot.Middleware;

namespace NightTasker.ApiGateway.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureOcelot(this WebApplication app)
    {
        app.UseOcelot().Wait();
        return app;
    }
}
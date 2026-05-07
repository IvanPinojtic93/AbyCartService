using System.Reflection;

namespace CartService.API.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod("MapEndpoints", BindingFlags.Static | BindingFlags.Public);
            method?.Invoke(null, [app]);
        }

        return app;
    }
}

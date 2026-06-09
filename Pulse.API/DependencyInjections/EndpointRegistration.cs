using Pulse.API.Features;

namespace Pulse.API.DependencyInjections;

public static class EndpointRegistration
{
    public static void MapEndpoints(this WebApplication app) {

        var endpointTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        foreach (var type in endpointTypes)
        {
            var instance = Activator.CreateInstance(type) as IEndpoint;
            instance?.MapEndpoint(app);
        }

    }
}

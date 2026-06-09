using Pulse.API.Infrastructure;
using Pulse.API.Services;

namespace Pulse.API.DependencyInjections;

public static class ServiceDep
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IFileService, FileService>();
    }
}

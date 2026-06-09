using Pulse.API.Services;

namespace Pulse.API.DependencyInjections;

public static class NewsDep
{
    public static void AddNewsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<INewsService, NewsService>(client =>
        {
            client.BaseAddress = new Uri("https://newsapi.org/v2/");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Pulse-App/1.0");
        });

        services.AddMemoryCache();
    }
}

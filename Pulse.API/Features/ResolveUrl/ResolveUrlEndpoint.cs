namespace Pulse.API.Features.ResolveUrl;

/// <summary>
/// Resolves a shortened URL (e.g. maps.app.goo.gl) to its final destination
/// by following redirects server-side, bypassing browser CORS restrictions.
/// Used by the dashboard MapPicker to extract coordinates from Google Maps short links.
/// </summary>
public class ResolveUrlEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/resolve-url", async (string url, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(url))
                return Results.BadRequest("url is required");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "https" && uri.Scheme != "http"))
                return Results.BadRequest("Invalid URL");

            try
            {
                using var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 10,
                };
                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(8);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                // HEAD request is enough — we only need the final URL, not the body
                var request = new HttpRequestMessage(HttpMethod.Head, uri);
                var response = await client.SendAsync(request, ct);

                var finalUrl = response.RequestMessage?.RequestUri?.ToString() ?? url;
                return Results.Ok(new { url = finalUrl });
            }
            catch
            {
                return Results.Ok(new { url }); // return original if resolution fails
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}

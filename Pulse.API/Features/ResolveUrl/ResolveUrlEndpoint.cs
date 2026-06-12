using System.Text.RegularExpressions;

namespace Pulse.API.Features.ResolveUrl;

/// <summary>
/// Extracts coordinates from a Google Maps URL (full or shortened).
/// For short URLs (maps.app.goo.gl), reads the Location header from the
/// first redirect — before Google serves its bot-detection page — to get
/// the full maps URL containing /@lat,lng or !3d!4d params.
/// </summary>
public class ResolveUrlEndpoint : IEndpoint
{
    private static readonly Regex AtRegex   = new(@"/@(-?\d+\.?\d*),(-?\d+\.?\d*)");
    private static readonly Regex DataRegex = new(@"!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)");
    private static readonly Regex CenterRegex = new(@"center=(-?\d+\.?\d*)(?:%2C|,)(-?\d+\.?\d*)", RegexOptions.IgnoreCase);

    private static (double lat, double lng)? TryExtract(string text)
    {
        // !3d!4d is most precise (actual pin), try first
        var m = DataRegex.Match(text);
        if (m.Success) return (double.Parse(m.Groups[1].Value), double.Parse(m.Groups[2].Value));

        m = AtRegex.Match(text);
        if (m.Success) return (double.Parse(m.Groups[1].Value), double.Parse(m.Groups[2].Value));

        m = CenterRegex.Match(text);
        if (m.Success) return (double.Parse(m.Groups[1].Value), double.Parse(m.Groups[2].Value));

        return null;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/resolve-url", async (string url, ILogger<ResolveUrlEndpoint> logger, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(url))
                return Results.BadRequest("url is required");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "https" && uri.Scheme != "http"))
                return Results.BadRequest("Invalid URL");

            try
            {
                // Step 1: try to parse coords directly from the URL as given
                var direct = TryExtract(url);
                if (direct.HasValue)
                    return Results.Ok(new { lat = direct.Value.lat, lng = direct.Value.lng });

                // Step 2: follow ONE redirect manually — read the Location header.
                // maps.app.goo.gl redirects to google.com/maps/place/.../@lat,lng,zoom
                // We stop after the first redirect to avoid bot-detection pages.
                using var handler = new HttpClientHandler { AllowAutoRedirect = false };
                using var client  = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(8) };
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);

                // Check the Location header on the redirect response
                var location = response.Headers.Location?.ToString();
                logger.LogInformation("resolve-url: status={Status} location={Location}", (int)response.StatusCode, location);

                if (!string.IsNullOrEmpty(location))
                {
                    var fromLocation = TryExtract(location);
                    if (fromLocation.HasValue)
                        return Results.Ok(new { lat = fromLocation.Value.lat, lng = fromLocation.Value.lng });
                }

                // Step 3: follow all redirects and try the final URL
                using var handler2 = new HttpClientHandler { AllowAutoRedirect = true, MaxAutomaticRedirections = 10 };
                using var client2  = new HttpClient(handler2) { Timeout = TimeSpan.FromSeconds(8) };
                client2.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
                client2.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                var resp2    = await client2.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
                var finalUrl = resp2.RequestMessage?.RequestUri?.ToString() ?? url;
                logger.LogInformation("resolve-url: finalUrl={FinalUrl}", finalUrl);

                var fromFinal = TryExtract(finalUrl);
                if (fromFinal.HasValue)
                    return Results.Ok(new { lat = fromFinal.Value.lat, lng = fromFinal.Value.lng });

                // Step 4: last resort — read page body
                var html = await resp2.Content.ReadAsStringAsync(ct);
                var fromHtml = TryExtract(html);
                if (fromHtml.HasValue)
                    return Results.Ok(new { lat = fromHtml.Value.lat, lng = fromHtml.Value.lng });

                logger.LogWarning("resolve-url: no coords found for {Url}. finalUrl={FinalUrl}", url, finalUrl);
                return Results.NotFound(new { error = "Could not extract coordinates" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "resolve-url: exception for url={Url}", url);
                return Results.NotFound(new { error = "Failed to resolve URL" });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}

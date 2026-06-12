using System.Text.RegularExpressions;
using System.Text.Json;

namespace Pulse.API.Features.ResolveUrl;

/// <summary>
/// Resolves a Google Maps URL (full or shortened) to coordinates.
///
/// Strategy (in order of precision):
/// 1. Parse coords directly from the input URL — handles full google.com/maps URLs.
/// 2. Follow the short-URL redirect and try to parse the Location header URL.
/// 3. Fetch the full page HTML and extract coords from the og:image meta tag
///    (center=lat,lng) — this works for both named places and dropped pins.
/// 4. Nominatim geocoding by place name — last resort for named places only.
/// </summary>
public class ResolveUrlEndpoint : IEndpoint
{
    // Most precise — actual pin location in data params
    private static readonly Regex DataRegex   = new(@"!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)");
    private static readonly Regex Pin8mRegex  = new(@"!8m2!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)");
    // Viewport center in URL path
    private static readonly Regex AtRegex     = new(@"/@(-?\d+\.?\d*),(-?\d+\.?\d*)");
    // og:image meta tag: center=lat,lng or center=lat%2Clng
    private static readonly Regex CenterRegex = new(@"center=(-?\d+\.?\d*)(?:%2C|,)(-?\d+\.?\d*)", RegexOptions.IgnoreCase);
    // Place name in redirect URL path
    private static readonly Regex PlaceNameRegex = new(@"/maps/place/([^/?#]+)");

    private static (double lat, double lng)? TryExtract(string text)
    {
        var m = Pin8mRegex.Match(text);
        if (m.Success) return Parse(m);
        m = DataRegex.Match(text);
        if (m.Success) return Parse(m);
        m = AtRegex.Match(text);
        if (m.Success) return Parse(m);
        m = CenterRegex.Match(text);
        if (m.Success) return Parse(m);
        return null;
    }

    private static (double lat, double lng) Parse(Match m) =>
        (double.Parse(m.Groups[1].Value), double.Parse(m.Groups[2].Value));

    private static HttpClient BuildClient(bool followRedirects)
    {
        var handler = new HttpClientHandler { AllowAutoRedirect = followRedirects, MaxAutomaticRedirections = 10 };
        var client  = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        return client;
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

            // ── Step 1: parse coords directly from the input URL ─────────────
            var direct = TryExtract(url);
            if (direct.HasValue)
            {
                logger.LogInformation("resolve-url: coords from input URL");
                return Ok(direct.Value);
            }

            // ── Step 2: follow first redirect, try Location header ────────────
            string? locationHeader = null;
            try
            {
                using var c = BuildClient(false);
                var resp = await c.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
                locationHeader = resp.Headers.Location?.ToString();
                logger.LogInformation("resolve-url: redirect status={S} location={L}", (int)resp.StatusCode, locationHeader);
            }
            catch (Exception ex) { logger.LogWarning(ex, "resolve-url: step2 failed"); }

            if (!string.IsNullOrEmpty(locationHeader))
            {
                var fromLoc = TryExtract(locationHeader);
                if (fromLoc.HasValue)
                {
                    logger.LogInformation("resolve-url: coords from Location header");
                    return Ok(fromLoc.Value);
                }
            }

            // ── Step 3: fetch full page HTML, extract center= from og:image ───
            // This is the most reliable path for goo.gl short links.
            // Google's HTML always contains og:image with center=lat,lng.
            try
            {
                using var c = BuildClient(true);
                var html = await c.GetStringAsync(uri, ct);
                logger.LogInformation("resolve-url: HTML length={L}", html.Length);

                var fromHtml = TryExtract(html);
                if (fromHtml.HasValue)
                {
                    logger.LogInformation("resolve-url: coords from HTML center= pattern");
                    return Ok(fromHtml.Value);
                }

                logger.LogWarning("resolve-url: no coords in HTML. Snippet: {S}", html[..Math.Min(200, html.Length)]);
            }
            catch (Exception ex) { logger.LogWarning(ex, "resolve-url: step3 HTML fetch failed"); }

            // ── Step 4: Nominatim by place name (last resort, named places only)
            if (!string.IsNullOrEmpty(locationHeader))
            {
                var placeMatch = PlaceNameRegex.Match(locationHeader);
                if (placeMatch.Success)
                {
                    var rawName  = placeMatch.Groups[1].Value;
                    var placeName = Uri.UnescapeDataString(rawName).Replace('+', ' ').Trim();
                    placeName     = Regex.Replace(placeName, @"\s+\d{5,}.*$", "").Trim();
                    logger.LogInformation("resolve-url: Nominatim for '{N}'", placeName);

                    var nom = await GeocodePlaceName(placeName, logger, ct);
                    if (nom.HasValue) return Ok(nom.Value);
                }
            }

            logger.LogWarning("resolve-url: all strategies failed for {U}", url);
            return Results.NotFound(new { error = "Could not extract coordinates" });
        }).RequireAuthorization("ManagerOrAdmin");
    }

    private static IResult Ok((double lat, double lng) c) =>
        Results.Ok(new { lat = c.lat, lng = c.lng });

    private static async Task<(double lat, double lng)?> GeocodePlaceName(
        string name, ILogger logger, CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(6) };
            client.DefaultRequestHeaders.Add("User-Agent", "PulseDashboard/1.0");
            var q    = Uri.EscapeDataString(name);
            var resp = await client.GetAsync(
                $"https://nominatim.openstreetmap.org/search?q={q}&format=json&limit=1", ct);
            var json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.GetArrayLength() > 0)
            {
                var f = doc.RootElement[0];
                return (double.Parse(f.GetProperty("lat").GetString()!),
                        double.Parse(f.GetProperty("lon").GetString()!));
            }
        }
        catch (Exception ex) { logger.LogWarning(ex, "Nominatim failed for '{N}'", name); }
        return null;
    }
}

using System.Text.RegularExpressions;
using System.Text.Json;

namespace Pulse.API.Features.ResolveUrl;

/// <summary>
/// Resolves a Google Maps URL (full or shortened) to coordinates.
///
/// Strategy:
/// 1. Try parsing coords directly from the URL (works for full maps URLs).
/// 2. For short URLs (maps.app.goo.gl): read the Location header from the first
///    redirect — if it contains /@lat,lng or !3d!4d, extract directly.
/// 3. If the redirect URL has no coords but has a place name, geocode that
///    place name using Nominatim (free, no API key required).
/// </summary>
public class ResolveUrlEndpoint : IEndpoint
{
    private static readonly Regex AtRegex     = new(@"/@(-?\d+\.?\d*),(-?\d+\.?\d*)");
    private static readonly Regex DataRegex   = new(@"!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)");
    private static readonly Regex CenterRegex = new(@"center=(-?\d+\.?\d*)(?:%2C|,)(-?\d+\.?\d*)", RegexOptions.IgnoreCase);
    // Matches the place name segment in /maps/place/PLACE_NAME/
    private static readonly Regex PlaceNameRegex = new(@"/maps/place/([^/?]+)");

    private static (double lat, double lng)? TryExtract(string text)
    {
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

            // Step 1: try to parse coords directly from the input URL
            var direct = TryExtract(url);
            if (direct.HasValue)
                return Results.Ok(new { lat = direct.Value.lat, lng = direct.Value.lng });

            using var httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
            {
                Timeout = TimeSpan.FromSeconds(8)
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

            // Step 2: follow one redirect, read Location header
            string? locationHeader = null;
            try
            {
                var resp = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
                locationHeader = resp.Headers.Location?.ToString();
                logger.LogInformation("resolve-url: status={Status} location={Location}", (int)resp.StatusCode, locationHeader);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "resolve-url: redirect step failed for {Url}", url);
            }

            if (!string.IsNullOrEmpty(locationHeader))
            {
                // Try extracting coords from the redirect URL
                var fromLocation = TryExtract(locationHeader);
                if (fromLocation.HasValue)
                    return Results.Ok(new { lat = fromLocation.Value.lat, lng = fromLocation.Value.lng });

                // No coords in the redirect URL — extract the place name and geocode it
                var placeMatch = PlaceNameRegex.Match(locationHeader);
                if (placeMatch.Success)
                {
                    var rawName = placeMatch.Groups[1].Value;
                    // URL-decode and clean up (replace + with space)
                    var placeName = Uri.UnescapeDataString(rawName).Replace('+', ' ').Trim();
                    // Strip trailing numeric postal codes (e.g. "كفر الشيخ 6973101‭")
                    placeName = Regex.Replace(placeName, @"\s+\d{5,}.*$", "").Trim();

                    logger.LogInformation("resolve-url: geocoding place name: {PlaceName}", placeName);

                    var nominatimResult = await GeocodePlaceName(placeName, logger, ct);
                    if (nominatimResult.HasValue)
                        return Results.Ok(new { lat = nominatimResult.Value.lat, lng = nominatimResult.Value.lng });
                }
            }

            logger.LogWarning("resolve-url: all strategies failed for {Url}", url);
            return Results.NotFound(new { error = "Could not extract coordinates" });
        }).RequireAuthorization("ManagerOrAdmin");
    }

    private static async Task<(double lat, double lng)?> GeocodePlaceName(
        string placeName, ILogger logger, CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(6) };
            client.DefaultRequestHeaders.Add("User-Agent", "PulseDashboard/1.0 (location resolver)");

            var query = Uri.EscapeDataString(placeName);
            var nominatimUrl = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&limit=1&addressdetails=0";

            var response = await client.GetAsync(nominatimUrl, ct);
            var json = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement;

            if (results.GetArrayLength() > 0)
            {
                var first = results[0];
                var lat = double.Parse(first.GetProperty("lat").GetString()!);
                var lng = double.Parse(first.GetProperty("lon").GetString()!);
                logger.LogInformation("resolve-url: Nominatim found {Lat},{Lng} for '{Name}'", lat, lng, placeName);
                return (lat, lng);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "resolve-url: Nominatim geocoding failed for '{PlaceName}'", placeName);
        }

        return null;
    }
}

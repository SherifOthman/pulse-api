using System.Text.RegularExpressions;

namespace Pulse.API.Features.ResolveUrl;

/// <summary>
/// Resolves a shortened Google Maps URL (e.g. maps.app.goo.gl) to coordinates.
/// Short URLs serve an HTML page directly rather than redirecting to a standard
/// maps URL, so we fetch the page and extract coordinates from the og:image meta tag
/// which always contains "center=lat,lng".
/// </summary>
public class ResolveUrlEndpoint : IEndpoint
{
    // Matches: center=31.41677075%2C31.1296  or  center=31.41677075,31.1296
    private static readonly Regex CenterRegex =
        new(@"center=(-?\d+\.?\d*)(?:%2C|,)(-?\d+\.?\d*)", RegexOptions.IgnoreCase);

    // Fallback: /@lat,lng,zoom in a full maps URL after redirect
    private static readonly Regex AtRegex =
        new(@"/@(-?\d+\.?\d*),(-?\d+\.?\d*)");

    // Fallback: !3d...!4d... data param
    private static readonly Regex DataRegex =
        new(@"!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)");

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
                using var handler = new HttpClientHandler { AllowAutoRedirect = true, MaxAutomaticRedirections = 10 };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(8) };
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var html = await client.GetStringAsync(uri, ct);

                // 1. og:image contains center=lat%2Clng — most reliable for short URLs
                var centerMatch = CenterRegex.Match(html);
                if (centerMatch.Success)
                    return Results.Ok(new
                    {
                        lat = double.Parse(centerMatch.Groups[1].Value),
                        lng = double.Parse(centerMatch.Groups[2].Value),
                    });

                // 2. Full maps URL in the page (canonical link or href)
                var atMatch = AtRegex.Match(html);
                if (atMatch.Success)
                    return Results.Ok(new
                    {
                        lat = double.Parse(atMatch.Groups[1].Value),
                        lng = double.Parse(atMatch.Groups[2].Value),
                    });

                // 3. !3d!4d data params
                var dataMatch = DataRegex.Match(html);
                if (dataMatch.Success)
                    return Results.Ok(new
                    {
                        lat = double.Parse(dataMatch.Groups[1].Value),
                        lng = double.Parse(dataMatch.Groups[2].Value),
                    });

                return Results.NotFound(new { error = "Could not extract coordinates from URL" });
            }
            catch
            {
                return Results.NotFound(new { error = "Failed to resolve URL" });
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}

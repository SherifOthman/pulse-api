namespace Pulse.API.Infrastructure;

/// <summary>
/// Converts a stored relative image path (e.g. /uploads/businesses/xxx.jpg)
/// into a fully-qualified URL using the server base URL passed from the endpoint.
/// This ensures mobile devices always get a reachable URL regardless of
/// which IP/hostname the server is running on.
/// </summary>
public static class UrlHelper
{
    /// <summary>
    /// If path is already absolute return as-is; otherwise prefix with baseUrl.
    /// Returns null when path is null or whitespace.
    /// </summary>
    public static string? ToAbsolute(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        if (path.StartsWith("http://") || path.StartsWith("https://")) return path;
        return $"{baseUrl}{(path.StartsWith('/') ? path : '/' + path)}";
    }
}

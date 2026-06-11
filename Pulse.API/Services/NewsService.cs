using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Pulse.API.Features.Shared;

namespace Pulse.API.Services;

public class NewsService : INewsService
{
    private const string CacheKey = "news_articles";

    // Keyword rules to classify Arabic articles into categories
    private static readonly (string[] Keywords, string Category)[] CategoryRules =
    [
        (["صيدل", "دواء", "أدوية", "علاج دوائ", "لقاح", "pharmacy", "drug", "vaccine"], "أدوية"),
        (["تغذية", "رجيم", "سعرات", "فيتامين", "بروتين", "nutrition", "diet", "vitamin"], "تغذية"),
    ];
    private const string DefaultCategory = "طب";

    private readonly HttpClient    _httpClient;
    private readonly IMemoryCache  _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<NewsService> _logger;

    public NewsService(
        HttpClient httpClient,
        IMemoryCache cache,
        IConfiguration config,
        ILogger<NewsService> logger)
    {
        _httpClient = httpClient;
        _cache      = cache;
        _config     = config;
        _logger     = logger;
    }

    // ── Public API ──────────────────────────────────────────────────────────

    public async Task<PaginatedResponse<NewsArticleDto>> GetNewsAsync(
        int page = 1, int pageSize = 10,
        string? category = null, string? search = null,
        CancellationToken ct = default)
    {
        var articles = await GetOrFetchAsync(ct);

        var filtered = articles.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
            filtered = filtered.Where(a => a.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.ToLowerInvariant();
            filtered = filtered.Where(a =>
                a.Title.ToLowerInvariant().Contains(q) ||
                a.Summary.ToLowerInvariant().Contains(q));
        }

        var list  = filtered.OrderByDescending(a => a.PublishedAt).ToList();
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(Map).ToList();

        return new PaginatedResponse<NewsArticleDto>(items, page, pageSize, list.Count, page * pageSize < list.Count);
    }

    public async Task<List<string>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var articles = await GetOrFetchAsync(ct);
        return articles.Select(a => a.Category).Distinct().OrderBy(c => c).ToList();
    }

    public async Task<NewsArticleDto?> GetNewsByIdAsync(string id, CancellationToken ct = default)
    {
        var articles = await GetOrFetchAsync(ct);
        var a = articles.FirstOrDefault(x => x.Id == id);
        return a is null ? null : Map(a);
    }

    // ── Fetch & cache ────────────────────────────────────────────────────────

    private async Task<List<Article>> GetOrFetchAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(CacheKey, out List<Article>? cached) && cached is { Count: > 0 })
        {
            _logger.LogInformation("News served from cache ({Count} articles).", cached.Count);
            return cached;
        }

        var apiKey = _config["NewsApi:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("NewsApi:ApiKey is not configured.");
            return [];
        }

        _logger.LogInformation("News cache miss — fetching from NewsAPI...");

        var articles = await FetchAsync(apiKey, ct);
        if (articles.Count == 0)
        {
            _logger.LogWarning("No articles returned from NewsAPI.");
            return [];
        }

        var minutes = _config.GetValue<int>("NewsApi:CacheMinutes", 60);
        _cache.Set(CacheKey, articles, TimeSpan.FromMinutes(minutes));
        _logger.LogInformation("Cached {Count} articles for {Minutes} min.", articles.Count, minutes);

        return articles;
    }

    private async Task<List<Article>> FetchAsync(string apiKey, CancellationToken ct)
    {
        // searchIn=title ensures only articles whose TITLE contains the medical terms
        // are returned — prevents tech/entertainment articles that mention "health"
        // in passing from appearing. Arabic keywords cover the primary categories.
        var query = Uri.EscapeDataString(
            "صحة OR طب OR دواء OR مستشفى OR تغذية OR فيتامين OR علاج OR مرض OR صيدلية OR أشعة");
        var url = $"everything?q={query}&searchIn=title&language=ar&pageSize=100&sortBy=publishedAt&apiKey={apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("NewsAPI {Status}: {Body}", (int)response.StatusCode, body);
                return [];
            }

            var json    = await response.Content.ReadAsStringAsync(ct);
            var wrapper = JsonSerializer.Deserialize<ApiResponse>(json, JsonOpts);

            _logger.LogInformation("NewsAPI returned {Count} articles.", wrapper?.Articles?.Count ?? 0);

            return wrapper?.Articles?
                .Where(a => !string.IsNullOrEmpty(a.Title)
                         && !string.IsNullOrEmpty(a.Url)
                         && a.Title != "[Removed]")
                .Select(a => new Article
                {
                    Id          = StableId(a.Url!),
                    Title       = Clip(a.Title,       300),
                    Summary     = Clip(a.Description, 1000),
                    Content     = "",   // NewsAPI free-tier content is stripped HTML — not usable
                    ImageUrl    = a.UrlToImage,
                    Category    = Classify(a.Title + " " + a.Description),
                    Author      = Clip(a.Author ?? a.Source?.Name, 200),
                    SourceUrl   = a.Url,
                    PublishedAt = ParseDate(a.PublishedAt),
                })
                .ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception fetching news from NewsAPI.");
            return [];
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Classifies an article into an Arabic category using keyword matching.</summary>
    private static string Classify(string? text)
    {
        if (string.IsNullOrEmpty(text)) return DefaultCategory;
        foreach (var (keywords, category) in CategoryRules)
            if (keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase)))
                return category;
        return DefaultCategory;
    }

    private static NewsArticleDto Map(Article a) => new(
        a.Id, a.Title, a.Summary, a.Content,
        a.ImageUrl, a.Category, a.PublishedAt.ToString("o"),
        a.Author, IsFeatured: false, SourceUrl: a.SourceUrl);

    private static string StableId(string url)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(url));
        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }

    private static string Clip(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Length <= max ? value : value[..max];
    }

    private static DateTime ParseDate(string? s) =>
        DateTime.TryParse(s, out var d) ? d : DateTime.UtcNow;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    };

    // ── Internal models ──────────────────────────────────────────────────────

    private sealed class Article
    {
        public string   Id          { get; set; } = "";
        public string   Title       { get; set; } = "";
        public string   Summary     { get; set; } = "";
        public string   Content     { get; set; } = "";
        public string?  ImageUrl    { get; set; }
        public string   Category    { get; set; } = "";
        public string?  Author      { get; set; }
        public string?  SourceUrl   { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    private sealed class ApiResponse
    {
        public string?           Status       { get; set; }
        public int               TotalResults { get; set; }
        public List<ApiArticle>? Articles     { get; set; }
    }

    private sealed class ApiArticle
    {
        public ApiSource? Source      { get; set; }
        public string?    Author      { get; set; }
        public string?    Title       { get; set; }
        public string?    Description { get; set; }
        public string?    Url         { get; set; }
        public string?    UrlToImage  { get; set; }
        public string?    PublishedAt { get; set; }
        public string?    Content     { get; set; }
    }

    private sealed class ApiSource
    {
        public string? Id   { get; set; }
        public string? Name { get; set; }
    }
}


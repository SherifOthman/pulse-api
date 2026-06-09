namespace Pulse.API.Services;

public interface INewsService
{
    Task<PaginatedNewsResponse> GetNewsAsync(int page = 1, int pageSize = 10, string? category = null, string? search = null, CancellationToken ct = default);
    Task<List<string>> GetCategoriesAsync(CancellationToken ct = default);
    Task<NewsArticleDto?> GetNewsByIdAsync(string id, CancellationToken ct = default);
}

public record NewsArticleDto(
    string Id,
    string Title,
    string Summary,
    string Content,
    string? ImageUrl,
    string Category,
    string PublishedAt,
    string? Author,
    bool IsFeatured,
    string? SourceUrl);

public record PaginatedNewsResponse(
    List<NewsArticleDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    bool HasMore);

using MediatR;
using Pulse.API.Features.Shared;
using Pulse.API.Services;

namespace Pulse.API.Features.News.GetNews;

public record GetNewsQuery(
    int Page,
    int PageSize,
    string? Category,
    string? Search
) : IRequest<PaginatedResponse<NewsArticleDto>>;

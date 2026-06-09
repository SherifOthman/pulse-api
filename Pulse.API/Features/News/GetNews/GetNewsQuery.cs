using MediatR;

namespace Pulse.API.Features.News.GetNews;

public record GetNewsQuery(
    int Page,
    int PageSize,
    string? Category,
    string? Search
) : IRequest<object>;

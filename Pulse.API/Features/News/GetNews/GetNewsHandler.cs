using Pulse.API.Features.Shared;
using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNews;

public class GetNewsHandler(INewsService newsService) : IRequestHandler<GetNewsQuery, PaginatedResponse<NewsArticleDto>>
{
    public async Task<PaginatedResponse<NewsArticleDto>> Handle(GetNewsQuery request, CancellationToken ct)
    {
        return await newsService.GetNewsAsync(request.Page, request.PageSize, request.Category, request.Search);
    }
}

using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNews;

public class GetNewsHandler(INewsService newsService) : IRequestHandler<GetNewsQuery, object>
{
    public async Task<object> Handle(GetNewsQuery request, CancellationToken ct)
    {
        return await newsService.GetNewsAsync(request.Page, request.PageSize, request.Category, request.Search);
    }
}

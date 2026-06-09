using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public class GetNewsCategoriesHandler(INewsService newsService) : IRequestHandler<GetNewsCategoriesQuery, object>
{
    public async Task<object> Handle(GetNewsCategoriesQuery request, CancellationToken ct)
    {
        return await newsService.GetCategoriesAsync();
    }
}

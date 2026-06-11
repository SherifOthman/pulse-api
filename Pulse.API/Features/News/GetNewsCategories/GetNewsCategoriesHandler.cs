using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public class GetNewsCategoriesHandler(INewsService newsService) : IRequestHandler<GetNewsCategoriesQuery, List<string>>
{
    public async Task<List<string>> Handle(GetNewsCategoriesQuery request, CancellationToken ct)
    {
        return await newsService.GetCategoriesAsync();
    }
}

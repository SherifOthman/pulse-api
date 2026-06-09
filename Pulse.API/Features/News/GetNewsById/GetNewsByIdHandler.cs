using Pulse.API.Services;
using MediatR;

namespace Pulse.API.Features.News.GetNewsById;

public class GetNewsByIdHandler(INewsService newsService) : IRequestHandler<GetNewsByIdQuery, object?>
{
    public async Task<object?> Handle(GetNewsByIdQuery request, CancellationToken ct)
    {
        return await newsService.GetNewsByIdAsync(request.Id);
    }
}

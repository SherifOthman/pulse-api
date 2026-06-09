using MediatR;

namespace Pulse.API.Features.News.GetNewsById;

public record GetNewsByIdQuery(string Id) : IRequest<object?>;

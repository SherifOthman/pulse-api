using MediatR;
using Pulse.API.Services;

namespace Pulse.API.Features.News.GetNewsById;

public record GetNewsByIdQuery(string Id) : IRequest<NewsArticleDto?>;

using MediatR;

namespace Pulse.API.Features.News.GetNewsCategories;

public record GetNewsCategoriesQuery : IRequest<List<string>>;

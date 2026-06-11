using MediatR;

namespace Pulse.API.Features.Favorites.GetFavorites;

public record GetFavoritesQuery(string? BaseUrl = null) : IRequest<List<FavoriteListItemResponse>>;

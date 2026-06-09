using MediatR;

namespace Pulse.API.Features.Favorites.GetFavorites;

public record GetFavoritesQuery : IRequest<List<FavoriteListItemResponse>>;

using MediatR;

namespace Pulse.API.Features.Favorites.ToggleFavorite;

public record ToggleFavoriteCommand(Guid BusinessId) : IRequest;

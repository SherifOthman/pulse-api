using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Favorites.ToggleFavorite;

public class ToggleFavoriteHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<ToggleFavoriteCommand>
{
    public async Task Handle(ToggleFavoriteCommand request, CancellationToken ct)
    {
        var existing = await db.UserFavorite
            .FirstOrDefaultAsync(f => f.UserId == currentUser.Id && f.BuissnessId == request.BusinessId, ct);

        if (existing is not null)
            db.UserFavorite.Remove(existing);
        else
            db.UserFavorite.Add(new UserFavorite
            {
                UserId = currentUser.Id,
                BuissnessId = request.BusinessId
            });

        await db.SaveChangesAsync(ct);
    }
}

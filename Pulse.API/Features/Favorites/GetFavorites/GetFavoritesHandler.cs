using Pulse.API.Infrastructure;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Favorites.GetFavorites;

public class GetFavoritesHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetFavoritesQuery, List<FavoriteListItemResponse>>
{
    public async Task<List<FavoriteListItemResponse>> Handle(GetFavoritesQuery request, CancellationToken ct)
    {
        return await db.UserFavorite
            .AsNoTracking()
            .Where(f => f.UserId == currentUser.Id)
            .Select(f => new FavoriteListItemResponse(
                f.BusinessId,
                f.Business.Name,
                f.Business.ProfileImageUrl,
                f.Business.Testimonials
                    .Select(t => (double)t.Rating)
                    .DefaultIfEmpty()
                    .Average(),
                f.Business.Testimonials.Count,
                (int)f.Business.Type
            ))
            .ToListAsync(ct)
            .ContinueWith(t => t.Result
                .OrderBy(f => f.BusinessType)
                .ThenBy(f => f.Name)
                .ToList(), ct);
    }
}

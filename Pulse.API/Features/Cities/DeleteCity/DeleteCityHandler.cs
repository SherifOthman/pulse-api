using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;

namespace Pulse.API.Features.Cities.DeleteCity;

public class DeleteCityHandler(AppDbContext db)
    : IRequestHandler<DeleteCityCommand>
{
    public async Task Handle(DeleteCityCommand request, CancellationToken ct)
    {
        var city = await db.Set<City>().FindAsync([request.Id], ct);
        if (city is null)
            throw new KeyNotFoundException("City not found");

        db.Set<City>().Remove(city);
        await db.SaveChangesAsync(ct);
    }
}

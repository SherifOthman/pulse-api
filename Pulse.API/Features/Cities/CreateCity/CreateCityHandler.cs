using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Cities.CreateCity;

public class CreateCityHandler(AppDbContext db)
    : IRequestHandler<CreateCityCommand, CityResponse>
{
    public async Task<CityResponse> Handle(CreateCityCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadHttpRequestException("Name is required");

        var governorateExists = await db.Set<Governorate>().AnyAsync(g => g.Id == request.GovernorateId, ct);
        if (!governorateExists)
            throw new BadHttpRequestException("Governorate not found");

        var city = new City
        {
            GovernorateId = request.GovernorateId,
            Name = request.Name.Trim(),
        };

        db.Set<City>().Add(city);
        await db.SaveChangesAsync(ct);

        return new CityResponse(city.Id, city.GovernorateId, city.Name);
    }
}

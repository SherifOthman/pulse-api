using Pulse.API.Domain.Entities;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Cities.UpdateCity;

public class UpdateCityHandler(AppDbContext db)
    : IRequestHandler<UpdateCityCommand, CityResponse>
{
    public async Task<CityResponse> Handle(UpdateCityCommand request, CancellationToken ct)
    {
        var city = await db.Set<City>().FindAsync([request.Id], ct);
        if (city is null)
            throw new KeyNotFoundException("City not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            city.Name = request.Name.Trim();

        if (request.GovernorateId.HasValue)
        {
            var govExists = await db.Set<Governorate>().AnyAsync(g => g.Id == request.GovernorateId.Value, ct);
            if (!govExists)
                throw new BadHttpRequestException("Governorate not found");
            city.GovernorateId = request.GovernorateId.Value;
        }

        await db.SaveChangesAsync(ct);
        return new CityResponse(city.Id, city.GovernorateId, city.Name);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Radiology.UpdateRadiology;

public class UpdateRadiologyHandler(AppDbContext db)
    : IRequestHandler<UpdateRadiologyCommand, RadiologyResponse>
{
    public async Task<RadiologyResponse> Handle(UpdateRadiologyCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Radiology, ct);

        if (business is null)
            throw new NotFoundException("Radiology center not found");

        await BusinessMappingHelpers.ApplyUpdatesAsync(
            db, business, request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude, ct);

        await db.SaveChangesAsync(ct);
        return new RadiologyResponse(business.Id, business.Name);
    }
}

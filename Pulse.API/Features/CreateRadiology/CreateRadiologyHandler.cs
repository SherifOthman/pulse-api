using MediatR;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Radiology.CreateRadiology;

public class CreateRadiologyHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateRadiologyCommand, RadiologyResponse>
{
    public async Task<RadiologyResponse> Handle(CreateRadiologyCommand request, CancellationToken ct)
    {
        var business = await BusinessMappingHelpers.CreateBusinessAsync(
            db, request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude,
            BusinessType.Radiology, currentUser.Id, ct);

        business.RadiologyProfile = new RadiologyProfile();

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        if (request.Services is { Count: > 0 })
            await BusinessServiceHelpers.LinkServicesAsync(
                db, business.Id, BusinessType.Radiology, request.Services, ct);

        return new RadiologyResponse(business.Id, business.Name);
    }
}
